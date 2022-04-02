using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.Solutions;
using Olympiad.Services.SolutionCheckQueue;
using Olympiad.Shared;
using Olympiad.Shared.Extensions;
using Olympiad.Shared.Models;
using PublicAPI.Requests.Solutions;
using PublicAPI.Responses.Solutions;
using WebApp.Extensions;
using WebApp.Models;
using WebApp.Services.Attachments;
using WebApp.Services.Interfaces;
using WebApp.Services.Solutions;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Check")]
    [Authorize(Roles = "User")]
    public class CheckController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly ISolutionsService solutionsService;
        private readonly IMapper mapper;
        private readonly ILogger<CheckController> logger;

        public CheckController(
            ApplicationDbContext context,
            ISolutionsService solutionsService,
            UserManager<User> userManager,
            IMapper mapper,
            ILogger<CheckController> logger) : base(userManager)
        {
            this.context = context;
            this.solutionsService = solutionsService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost("code/{exerciseId}/{language}")]
        public async Task<ActionResult<SolutionResponse>> Post(IFormFile file, string language, Guid exerciseId)
        {
            return await AddSolutionFromFile(file, language, exerciseId, UserId);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("code/{exerciseId}/{language}/{authorId}")]
        public async Task<ActionResult<SolutionResponse>> AdminPost(IFormFile file, string language, Guid exerciseId, Guid authorId)
        {
            return await AddSolutionFromFile(file, language, exerciseId, authorId, true);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("code/{exerciseId}/{language}/{authorId}/rawstring")]
        public async Task<ActionResult<SolutionResponse>> AdminPost(
            [FromBody] string raw, string language, Guid exerciseId, Guid authorId)
        {
            return await AddSolutionFromStringRaw(raw, language, exerciseId, authorId, true);
        }
        private async Task<ActionResult<SolutionResponse>> AddSolutionFromFile(IFormFile file, string language, Guid exerciseId, Guid authorId, bool isAdmin = false)
        {
            if (file == null)
            {
                logger.LogInformation("File is null");
                return BadRequest("file not exists");
            }

            if (file.Length > 5_000_000)
            {
                logger.LogInformation($"File size too big {file.Length}");
                return BadRequest("file size more than 5MB");
            }
            string fileBody;
            using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                fileBody = await streamReader.ReadToEndAsync();
                if (!fileBody.IsLegalUnicode())
                {
                    logger.LogInformation($"File contains invalid Unicode");
                    return BadRequest($"The file contains invalid Unicode");
                }
            }
            return await AddSolutionFromStringRaw(fileBody, language, exerciseId, authorId, isAdmin);
        }
        private async Task<ActionResult<SolutionResponse>> AddSolutionFromStringRaw(string fileBody, string language, Guid exerciseId, Guid authorId, bool isAdmin = false)
        {
            if (!ProgramRuntime.TryFromValue(language, out var runtime))
            {
                return BadRequest("Incorrect target runtime (language)");
            }
            try
            {
                var checks = isAdmin ?
                    ISolutionsService.CodeSolutionChecks.None :
                    ISolutionsService.CodeSolutionChecks.All;
                var postedSolution = await solutionsService.PostCodeSolution(fileBody, runtime, exerciseId, authorId, checks);
                return mapper.Map<SolutionResponse>(mapper.Map<SolutionInternalModel>(postedSolution));
            }
            catch (ISolutionsService.ChallengeNotAvailableException)
            {
                return Forbid("Target challenge not available");
            }
            catch (ISolutionsService.TooManyPostException tre)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, tre.Message);
            }
            catch (ISolutionsService.AlreadySentException)
            {
                return Conflict("Solution already sent");
            }
            catch (ISolutionsService.ExerciseRuntimesRestrictionException erex)
            {
                return Conflict(new
                {
                    Message = "exercise restrinctions conflict",
                    AllowedRuntimes = erex.AllowerRuntimes
                });
            }
            catch (ISolutionsService.NotFoundEntityException nfe)
            {
                return NotFound(nfe.Message);
            }
        }



        [HttpPost("docs/{exerciseId}")]
        public async Task<ActionResult<CreatedDocsExerciseSolutionResponse>> PostDocsSolution(
            Guid exerciseId,
            [FromBody] DocsExerciseSolutionRequest request)
        {
            try
            {
                var result = await solutionsService.PostDocsSolution(exerciseId, UserId, request.Files, ISolutionsService.CodeSolutionChecks.All);
                var solutionResponse = mapper.Map<SolutionResponse>(mapper.Map<SolutionInternalModel>(result.solution));
                // TODO: handmade
                solutionResponse.Documents = result.solution.DocumentsResult.Files.Select(mapper.Map<SolutionDocumentResponse>).ToList();
                return new CreatedDocsExerciseSolutionResponse
                {
                    Solution = solutionResponse,
                    UploadUrls = result.uploadUrls
                };
            }
            catch (ISolutionsService.ChallengeNotAvailableException)
            {
                return Forbid("Target challenge not available");
            }
            catch (ISolutionsService.TooManyPostException tre)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, tre.Message);
            }
            catch (ISolutionsService.IncorrectFilesException)
            {
                return Conflict("Incorrect files");
            }
        }


        [HttpGet]
        public async Task<List<SolutionResponse>> Get()
        {
            var solutionResonses = await context
               .Solutions
               .Where(s => s.UserId == UserId)
               .ProjectTo<SolutionInternalModel>(mapper.ConfigurationProvider)
               .ToListAsync();
            return solutionResonses
                .Select(sim => mapper.Map<SolutionResponse>(sim))
                .ToList();
        }

        [HttpGet("forExercise")]
        public async Task<List<SolutionResponse>> GetForExercise(Guid exerciseId)
        {
            using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
            var solutionResonses = await context
               .Solutions
               .Where(s => s.UserId == UserId)
               .Where(s => s.ExerciseId == exerciseId)
               .OrderBy(s => s.SendingTime)
               .ProjectTo<SolutionInternalModel>(mapper.ConfigurationProvider)
               .ToListAsync();
            // TODO: handmade jsonb work
            var solutionDocs = await context
                .Solutions
                .Where(s => s.UserId == UserId)
                .Where(s => s.ExerciseId == exerciseId)
                .OrderBy(s => s.SendingTime)
                .Select(s => s.DocumentsResult)
                .ToListAsync();
            return solutionResonses
                .Select(sim => mapper.Map<SolutionResponse>(sim))
                .Zip(solutionDocs, (sim, docs) => { sim.Documents = docs?.Files?.Select(mapper.Map<SolutionDocumentResponse>).ToList(); return sim; })
                .ToList();
        }

        [HttpGet("{solutionId:guid}")]
        public async Task<SolutionResponse> Get(Guid solutionId)
        {
            var solutionInternal = await context
                .Solutions
                .Where(p => p.Id == solutionId && p.UserId == UserId)
                .ProjectTo<SolutionInternalModel>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;

            return mapper.Map<SolutionResponse>(solutionInternal);
        }
        [AllowAnonymous]
        [HttpGet("{solutionId:guid}/document/{fileName}")]
        public async Task<ActionResult> Get(Guid solutionId, string fileName, [FromServices] IAttachmentsService attachmentsService)
        {
            return Redirect(attachmentsService.GetUrlForSolutionDocument(solutionId, fileName));
        }

        [HttpGet]
        [Route("buildlogs/{solutionId:guid}")]
        public async Task<List<string>> GetBuildLogs(Guid solutionId)
        {
            var buildLogs = await context
                .SolutionBuildLogs
                .Where(p => p.SolutionId == solutionId && p.Solution.UserId == UserId)
                .Select(p => p.PrettyLog)
                .ToListAsync()
                ?? throw StatusCodeException.NotFount;

            return buildLogs;
        }


        [Authorize(Roles = RoleNames.EXECUTOR + "," + RoleNames.ADMIN)]
        [HttpGet("statistic")]
        public Task<List<SolutionsStatisticResponse>> GetStatistic()
        {
            return context
                .Solutions
                .GroupBy(s => s.Status)
                .Select(g => new SolutionsStatisticResponse { SolutionStatus = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();
        }

        [HttpGet("download/{solutionId}")]
        public async Task<IActionResult> Download(Guid solutionId)
        {
            var solutions = context
                .Solutions
                .Where(s => s.Id == solutionId);

            if (!IsAdmin && !User.IsResultsViewer())
            {
                solutions = solutions.Where(s => s.UserId == UserId);
            }

            var solution = await solutions
                .Select(s => new { s.Language, s.Raw, ExerciseType = s.Exercise.Type })
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
            if (solution.ExerciseType != ExerciseType.Code)
            {
                return BadRequest("Can't get solution raw for not code type exercise");
            }
            var solutionContent = Encoding.UTF8.GetBytes(solution.Raw);

            return File(solutionContent, "application/octet-stream", $"Program{ProgramRuntime.GetFileExtensionForRuntime(solution.Language)}");
        }
    }
}