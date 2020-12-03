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
using Microsoft.Extensions.Options;
using Models;
using Models.Solutions;
using Olympiad.Shared.Models;
using Olympiad.Shared.Models.Settings;
using PublicAPI.Responses;
using PublicAPI.Responses.Dump;
using PublicAPI.Responses.Solutions;
using WebApp.Extensions;
using WebApp.Models;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Check")]
    [Authorize(Roles = "User")]
    public class CheckController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly IQueueChecker queue;
        private readonly IMapper mapper;

        public CheckController(
            ApplicationDbContext context,
            IQueueChecker queue,
            UserManager<User> userManager,
            IMapper mapper) : base(userManager)
        {
            this.context = context;
            this.queue = queue;
            this.mapper = mapper;
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
                throw StatusCodeException.BadRequest("file not exists");
            }

            if (file.Length > 5120)
            {
                throw StatusCodeException.BadRequest("file size more than 5MB");
            }
            string fileBody;
            using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                fileBody = await streamReader.ReadToEndAsync();
                if (!fileBody.IsLegalUnicode())
                {
                    return BadRequest($"The file contains invalid Unicode");
                }
            }
            return await AddSolutionFromStringRaw(fileBody, language, exerciseId, authorId, isAdmin);
        }
        private async Task<ActionResult<SolutionResponse>> AddSolutionFromStringRaw(string fileBody, string language, Guid exerciseId, Guid authorId, bool isAdmin = false)
        {
            if (!await context.Exercises.AnyAsync(
                e => e.ExerciseID == exerciseId &&
                (e.Challenge.StartTime == null || e.Challenge.StartTime <= Now) &&
                (e.Challenge.EndTime == null || e.Challenge.EndTime >= Now)))
            {
                throw StatusCodeException.Conflict("Not found started challenge and exercise");
            }

            if (!isAdmin)
            {
                var lastSendingDate = (await context
                    .Solutions
                    .Where(s => s.UserId == authorId)
                    .Where(s => s.ExerciseId == exerciseId)
                    .Select(s => s.SendingTime)
                    .ToListAsync())
                    .DefaultIfEmpty(DateTimeOffset.MinValue)
                    .Max();

                if ((Now - lastSendingDate) < TimeSpan.FromMinutes(1))
                {
                    throw StatusCodeException.TooManyRequests;
                }

                var oldSolution = await context
                    .Solutions
                    .Where(s => s.UserId == authorId)
                    .Where(s => s.Raw == fileBody)
                    .Where(s => s.ExerciseId == exerciseId)
                    .FirstOrDefaultAsync();
                if (oldSolution != null)
                {
                    return mapper.Map<SolutionResponse>(mapper.Map<SolutionInternalModel>(oldSolution));
                }
            }

            Solution solution = new Solution()
            {
                Raw = fileBody,
                Language = language,
                ExerciseId = exerciseId,
                UserId = authorId,
                Status = SolutionStatus.InQueue,
                SendingTime = DateTimeOffset.UtcNow
            };

            await context.Solutions.AddAsync(solution);
            await context.SaveChangesAsync();

            queue.PutInQueue(solution.Id);

            return mapper.Map<SolutionResponse>(mapper.Map<SolutionInternalModel>(solution));
        }

        [HttpPost("recheck/{exerciseId:guid}/adminPanel")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> RecheckSolutionsAdminPanel(
            Guid exerciseId,
            [FromServices] IOptions<AdminSettings> options
)
        {
            if (HttpContext.Request.Headers["Authorization"].ToString() != options.Value.SecurityKey)
                return NotFound();
            return await RecheckSolutions(exerciseId);
        }

        [HttpPost("recheck/{exerciseId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> RecheckSolutions(Guid exerciseId)
        {
            var solutions = await context
                .Solutions
                .Where(s => s.Status != SolutionStatus.CompileError)
                .Where(s => s.ExerciseId == exerciseId)
                .ToListAsync();

            solutions.ForEach(s => s.Status = SolutionStatus.InQueue);
            await context.SaveChangesAsync();
            solutions.ForEach(s => queue.PutInQueue(s.Id));
            return solutions.Count;
        }

        [HttpPost("rechecksolution/{solutionId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> RecheckSolution(Guid solutionId)
        {
            var solution = await context
                .Solutions
                .SingleAsync(s => s.Id == solutionId);

            solution.Status = SolutionStatus.InQueue;
            await context.SaveChangesAsync();
            queue.PutInQueue(solution.Id);
            return 1;
        }

        [HttpPost("recheckusersolution/{studentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> RecheckUserSolutions(string studentId)
        {
            var solutions = await context
                .Solutions
                .Include(s => s.SolutionChecks)
                .Include(s => s.SolutionBuildLogs)
                .Where(s => s.User.StudentID == studentId)
                .ToListAsync();


            context.SolutionChecks.RemoveRange(solutions.SelectMany(s => s.SolutionChecks));
            context.SolutionBuildLogs.RemoveRange(solutions.SelectMany(s => s.SolutionBuildLogs));

            solutions.ForEach(s => s.Status = SolutionStatus.InQueue);
            await context.SaveChangesAsync();
            solutions.ForEach(s => queue.PutInQueue(s.Id));
            return solutions.Count;
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

        [Authorize(Roles = "Admin,Executor")]
        [HttpGet("statistic")]
        public Task<List<SolutionsStatisticResponse>> GetStatistic()
        {
            return context
                .Solutions
                .GroupBy(s => s.Status)
                .Select(g => new SolutionsStatisticResponse { SolutionStatus = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();
        }

        [HttpGet]
        [Route("solutionList/{exerciseId:guid}/{studentId}")]
        [Authorize(Roles = "Admin,ResultsViewer")]
        public async Task<List<SolutionDumpView>> Get(Guid exerciseId, string studentId)
        {
            return await context
                       .Solutions
                       .Where(p => p.ExerciseId == exerciseId && p.User.StudentID == studentId)
                       .ProjectTo<SolutionDumpView>(mapper.ConfigurationProvider)
                       .ToListAsync()
                   ?? throw StatusCodeException.NotFount;
        }

        [HttpGet("download/{solutionId}")]
        public async Task<IActionResult> Download(Guid solutionId)
        {
            var solutions = context
                .Solutions
                .Where(s => s.Id == solutionId);
            if (!IsAdmin)
                solutions = solutions.Where(s => s.UserId == UserId);
            var solution = await solutions
                .Select(s => new { s.Language, s.Raw })
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
            var solutionContent = Encoding.UTF8.GetBytes(solution.Raw);
            return File(solutionContent, "application/octet-stream", $"Program{GetExtensionsForLanguage(solution.Language)}");
        }

        [HttpGet("logs/{solutionId}")]
        [Authorize(Roles = "Admin,ResultsViewer")]
        public async Task<ActionResult<List<SolutionCheckResponse>>> GetLogs(Guid solutionId)
        {
            var rawData = await context
                .SolutionChecks
                .Where(sc => sc.SolutionId == solutionId)
                .ProjectTo<SolutionCheckResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return rawData
                .GroupBy(sch => sch.ExampleIn)
                .Select(g => g.OrderBy(sch => sch.CheckedTime).Last())
                .ToList();
        }

        private static string GetExtensionsForLanguage(string language)
        {
            return language switch
            {
                "java" => ".java",
                "csharp" => ".cs",
                "pasabc" => ".pas",
                "c" => ".c",
                "cpp" => ".cpp",
                "python" => ".py",
                _ => "",
            };
        }
    }
}