using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
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

        public CheckController(
            ApplicationDbContext context,
            IQueueChecker queue,
            UserManager<User> userManager) : base(userManager)
        {
            this.context = context;
            this.queue = queue;
        }

        [HttpPost]
        [Route("{language}/{exerciseId}")]
        public async Task<IActionResult> Post(IFormFile file, string language, Guid exerciseId)
        {
            string fileBody;
            if (file == null || file.Length > 5120)
            {
                return BadRequest("Отсутствует файл или его размер превышает 5MB");
            }
            var stream = file.OpenReadStream();

            if (!context.Exercises.Any(p => p.ExerciseID == exerciseId))
            {
                return BadRequest();
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
                Time = DateTime.Now
            };

            await context.Solutions.AddAsync(solution);
            await context.SaveChangesAsync();
            queue.PutInQueue(solution.Id);
            return Content(solution.Id.ToString());
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(context.Solutions.Where(P => P.UserId == UserId).ToList());
        }

        [HttpGet]
        [Route("{solutionId}")]
        public IActionResult Get(Guid solutionId)
        {
            return Json(context.Solutions.FirstOrDefault(P => P.Id == solutionId && P.UserId == UserId));
        }
    }
}