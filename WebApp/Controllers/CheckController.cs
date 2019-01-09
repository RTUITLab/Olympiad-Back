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
            var stream = file.OpenReadStream();

            if (!context.Exercises.Any(p => p.ExerciseID == exerciseId))
            {
                throw StatusCodeException.BadRequest();
            }

            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                fileBody = await streamReader.ReadToEndAsync();
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

        [HttpGet]
        public async Task<IEnumerable<SolutionResponse>> Get()
        {
            return await context
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
                .SingleOrDefaultAsync();
        }

        [HttpGet("download/{solutionId}")]
        public async Task<IActionResult> Download(Guid solutionId)
        {
            var solution = await context
                .Solutions
                .Where(s => s.Id == solutionId && s.UserId == UserId)
                .SingleOrDefaultAsync();
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