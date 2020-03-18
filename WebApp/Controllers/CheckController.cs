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
using Models;
using Models.Solutions;
using Olympiad.Shared.Models;
using PublicAPI.Responses;
using PublicAPI.Responses.Dump;
using PublicAPI.Responses.Solutions;
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
        public async Task<SolutionResponse> Post(IFormFile file, string language, Guid exerciseId)
        {
            return await AddSolution(file, language, exerciseId, UserId);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("{language}/{exerciseId}/{authorId}")]
        public async Task<SolutionResponse> AdminPost(IFormFile file, string language, Guid exerciseId, Guid authorId)
        {
            return await AddSolution(file, language, exerciseId, authorId, true);
        }

        private async Task<SolutionResponse> AddSolution(IFormFile file, string language, Guid exerciseId, Guid authorId, bool isAdmin = false)
        {
            string fileBody;
            if (file == null || file.Length > 5120)
            {
                throw StatusCodeException.BadRequest("Отсутствует файл или его размер превышает 5MB");
            }

            if (!await context.Exercises.AnyAsync(
                e => e.ExerciseID == exerciseId &&
                (e.Challenge.StartTime == null || e.Challenge.StartTime <= Now) &&
                (e.Challenge.EndTime == null || e.Challenge.EndTime >= Now)))
            {
                throw StatusCodeException.BadRequest();
            }

            if (!isAdmin)
            {
                var lastSendingDate = (await context
                    .Solutions
                    .Where(s => s.UserId == authorId)
                    .Where(s => s.ExerciseId == exerciseId)
                    .Select(s => s.SendingTime)
                    .ToListAsync())
                    .DefaultIfEmpty(DateTime.MinValue)
                    .Max();

                if ((Now - lastSendingDate) < TimeSpan.FromMinutes(1))
                {
                    throw StatusCodeException.TooManyRequests;
                }
            }

            using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                fileBody = await streamReader.ReadToEndAsync();
            }

            var oldSolution = await context
                .Solutions
                .Where(s => s.UserId == authorId)
                .Where(s => s.Raw == fileBody)
                .Where(s => s.ExerciseId == exerciseId)
                .FirstOrDefaultAsync(); // TODO change to SingleOrDefault before database drop

            if (!isAdmin && oldSolution != null)
            {
                return mapper.Map<SolutionResponse>(oldSolution);
            }

            Solution solution = new Solution()
            {
                Raw = fileBody,
                Language = language,
                ExerciseId = exerciseId,
                UserId = authorId,
                Status = SolutionStatus.InQueue,
                SendingTime = DateTime.UtcNow
            };

            await context.Solutions.AddAsync(solution);
            Solution sol2 = null;
            if (solution.Language == "pasabc")
            {
                sol2 = new Solution()
                {
                    Raw = fileBody,
                    Language = "fpas",
                    ExerciseId = exerciseId,
                    UserId = authorId,
                    Status = SolutionStatus.InQueue,
                    SendingTime = DateTime.UtcNow
                };
                await context.Solutions.AddAsync(sol2);
            }

            await context.SaveChangesAsync();
            queue.PutInQueue(solution.Id);
            if (sol2 != null)
            {
                queue.PutInQueue(sol2.Id);
            }
            return mapper.Map<SolutionResponse>(solution);
        }

        [HttpPost("recheck/{exerciseId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RecheckSolutions(Guid exerciseId)
        {
            var solutions = await context
                .Solutions
                .Where(s => s.Status != SolutionStatus.CompileError)
                .Where(s => s.ExerciseId == exerciseId)
                .ToListAsync();

            solutions.ForEach(s => s.Status = SolutionStatus.InQueue);
            await context.SaveChangesAsync();
            solutions.ForEach(s => queue.PutInQueue(s.Id));
            return Json(solutions.Count);
        }

        [HttpPost("rechecksolution/{solutionId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RecheckSolution(Guid solutionId)
        {
            var solution = await context
                .Solutions
                .SingleAsync(s => s.Id == solutionId);

            solution.Status = SolutionStatus.InQueue;
            await context.SaveChangesAsync();
            queue.PutInQueue(solution.Id);
            return Json(1);
        }

        [HttpPost("recheckusersolution/{studentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RecheckUserSolutions(string studentId)
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
            return Json(solutions.Count);
        }

        [HttpGet]
        public Task<List<SolutionResponse>> Get()
        {
            return context
                .Solutions
                .Where(s => s.UserId == UserId)
                .ProjectTo<SolutionResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
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
        [Route("{solutionId:guid}")]
        public async Task<SolutionResponse> Get(Guid solutionId)
        {
            return await context
                .Solutions
                .Where(p => p.Id == solutionId && p.UserId == UserId)
                .ProjectTo<SolutionResponse>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
        }

        [HttpGet]
        [Route("solutionList/{exerciseId:guid}/{studentId}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
            switch (language)
            {
                case "java":
                    return ".java";
                case "csharp":
                    return ".cs";
                case "pasabc":
                    return ".pas";
                case "c":
                    return ".c";
                case "cpp":
                    return ".cpp";
                case "python":
                    return ".py";
                default:
                    return "";
            }
        }
    }
}