using System;
using System.Collections.Generic;
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
using PublicAPI.Responses.Solutions;
using WebApp.Extensions;
using WebApp.Models;
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
        private readonly IQueueChecker queue;
        private readonly IMapper mapper;
        private readonly ILogger<CheckController> logger;

        public CheckController(
            ApplicationDbContext context,
            ISolutionsService solutionsService,
            IQueueChecker queue,
            UserManager<User> userManager,
            IMapper mapper,
            ILogger<CheckController> logger) : base(userManager)
        {
            this.context = context;
            this.solutionsService = solutionsService;
            this.queue = queue;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost]
        [Route("{language}/{exerciseId}")]
        public async Task<ActionResult<SolutionResponse>> Post(IFormFile file, string language, Guid exerciseId)
        {
            return await AddSolutionFromFile(file, language, exerciseId, UserId);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("{language}/{exerciseId}/{authorId}")]
        public async Task<ActionResult<SolutionResponse>> AdminPost(IFormFile file, string language, Guid exerciseId, Guid authorId)
        {
            return await AddSolutionFromFile(file, language, exerciseId, authorId, true);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("rawstring/{language}/{exerciseId}/{authorId}")]
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
                var checks = isAdmin ? default :
                    ISolutionsService.PostSolutionChecks.AlreadySent |
                    ISolutionsService.PostSolutionChecks.ChallengeAvailable |
                    ISolutionsService.PostSolutionChecks.TooManyPost |
                    ISolutionsService.PostSolutionChecks.ExerciseRuntimeRestrictions;
                var postedSolution = await solutionsService.PostSolution(fileBody, runtime, exerciseId, authorId, checks);
                return mapper.Map<SolutionResponse>(mapper.Map<SolutionInternalModel>(postedSolution));
            }
            catch (ISolutionsService.AlreadySentException)
            {
                return Conflict("Solution already sent");
            }
            catch (ISolutionsService.ChallengeNotAvailableException)
            {
                return Forbid("Target challenge not available");
            }
            catch (ISolutionsService.TooManyPostException tre)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, tre.Message);
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
            var solutionResonses = await context
               .Solutions
               .Where(s => s.UserId == UserId)
               .Where(s => s.ExerciseId == exerciseId)
               .ProjectTo<SolutionInternalModel>(mapper.ConfigurationProvider)
               .ToListAsync();
            return solutionResonses
                .Select(sim => mapper.Map<SolutionResponse>(sim))
                .ToList();
        }

        [HttpGet]
        [Route("{solutionId:guid}")]
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
                .Select(s => new { s.Language, s.Raw })
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
            var solutionContent = Encoding.UTF8.GetBytes(solution.Raw);

            return File(solutionContent, "application/octet-stream", $"Program{ProgramRuntime.GetFileExtensionForRuntime(solution.Language)}");
        }
    }
}