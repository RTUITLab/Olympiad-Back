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
using PublicAPI.Responses;
using Shared.Models;
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

            var lastSendingDate = await context
                .Solutions
                .Where(s => s.UserId == UserId)
                .Where(s => s.ExerciseId == exerciseId)
                .Select(s => s.SendingTime)
                .DefaultIfEmpty(DateTime.MinValue)
                .MaxAsync();

            if ((Now - lastSendingDate) < TimeSpan.FromMinutes(1))
            {
                throw StatusCodeException.TooManyRequests;
            }

            using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                fileBody = await streamReader.ReadToEndAsync();
            }

            var oldSolution = await context
                .Solutions
                .Where(s => s.UserId == UserId)
                .Where(s => s.Raw == fileBody)
                .Where(s => s.ExerciseId == exerciseId)
                .FirstOrDefaultAsync(); // TODO change to SingleOrDefault before database drop

            if (oldSolution != null)
            {
                return Mapper.Map<SolutionResponse>(oldSolution);
            }

            Solution solution = new Solution()
            {
                Raw = fileBody,
                Language = language,
                ExerciseId = exerciseId,
                UserId = UserId,
                Status = SolutionStatus.InQueue,
                SendingTime = DateTime.UtcNow
            };

            await context.Solutions.AddAsync(solution);
            await context.SaveChangesAsync();
            queue.PutInQueue(solution.Id);
            return mapper.Map<SolutionResponse>(solution);
        }


        [HttpPost("recheck/{exerciseId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RecheckSolutions(Guid exerciseId)
        {
            var solutions = await context
                .Solutions
                .Where(s => s.ExerciseId == exerciseId)
                .ToListAsync();

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
                .ProjectTo<SolutionResponse>()
                .ToListAsync();
        }

        [HttpGet]
        [Route("{solutionId}")]
        public async Task<SolutionResponse> Get(Guid solutionId)
        {
            return await context
                .Solutions
                .Where(p => p.Id == solutionId && p.UserId == UserId)
                .ProjectTo<SolutionResponse>()
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
        }

        [HttpGet("download/{solutionId}")]
        public async Task<IActionResult> Download(Guid solutionId)
        {
            var solution = await context
                .Solutions
                .Where(s => s.Id == solutionId && s.UserId == UserId)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
            var solutionContent = Encoding.UTF8.GetBytes(solution.Raw);
            return File(solutionContent, "application/octet-stream", $"Program{GetExtensionsForLanguage(solution.Language)}");
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