using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/ExerciseData")]
    public class ExerciseDataController : AuthorizeController
    {
        private readonly ApplicationDbContext context;

        public ExerciseDataController(ApplicationDbContext context, UserManager<User> userManager) : base(userManager)
        {
            this.context = context;
        }


        [HttpPost]
        [Route("{exerciseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(IFormFile inFile, IFormFile outFile, Guid exerciseId)
        {
            if (inFile == null || outFile == null || inFile.Length > 5120 || outFile.Length > 5120)
            {
                return BadRequest("Отсутствует файл входных данных или исходных данных. Проверьте размер передаваемого файла. Он не должен превышать 5MB");
            }

            var inStream = inFile.OpenReadStream();
            var outStream = outFile.OpenReadStream();

            if (!context.Exercises.Any(P => P.ExerciseID == exerciseId))
            {
                return BadRequest();
            }

            ExerciseData exerciseData = new ExerciseData()
            {
                ExerciseId = exerciseId
            };

            using (var inStreamReader = new StreamReader(inStream, Encoding.UTF8))
            {
                exerciseData.InData = await inStreamReader.ReadToEndAsync();
            }

            using (var outStreamReader = new StreamReader(outStream, Encoding.UTF8))
            {
                exerciseData.OutData = await outStreamReader.ReadToEndAsync();
            }

            await context.TestData.AddAsync(exerciseData);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("{exerciseId}")]
        [Authorize(Roles = "Executor")]
        public IActionResult Get(Guid exerciseId)
        {
            return Json(context.TestData.Where(P => P.ExerciseId == exerciseId).ToList());
        }
    }
}