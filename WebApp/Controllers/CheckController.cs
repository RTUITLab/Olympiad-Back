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

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Check")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CheckController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;

        public CheckController(ApplicationDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("{language}/{exerciseId}")]
        public async Task<IActionResult> Post(IFormFile file, string language, Guid exerciseId)
        {
            string fileBody;
            var stream = file.OpenReadStream();
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                fileBody = await streamReader.ReadToEndAsync();
            }

            if (!context.Exercises.Any(p => p.ExerciseID == exerciseId))
            {
                return BadRequest();
            }

            Solution solution = new Solution()
            {
                Raw = fileBody,
                Language = language,
                ExerciseId = exerciseId,
                UserId = Guid.Parse(userManager.GetUserId(User)),
                Status = SolutionStatus.InQueue
            };

            await context.Solutions.AddAsync(solution);
            await context.SaveChangesAsync();

            return Content(solution.Id.ToString());
        }

        [HttpGet]
        public IActionResult Get()
        {
            var id = Guid.Parse(userManager.GetUserId(User));
            return Json(context.Solutions.Where(P => P.UserId == id).ToList());
        }

        [HttpGet]
        [Route("{solutionId}")]
        public IActionResult Get(Guid solutionId)
        {
            var id = Guid.Parse(userManager.GetUserId(User));
            return Json(context.Solutions.FirstOrDefault(P => P.Id == solutionId && P.UserId == id));
        }
    }
}