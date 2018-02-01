using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/ExerciseData")]
    public class ExerciseDataController : Controller
    {
        private readonly ApplicationDbContext context;

        public ExerciseDataController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        [Route("{exerciseId}")]
        public async Task<IActionResult> Post(IFormFile inFile, IFormFile outFile, Guid exerciseId)
        {
            var inStream = inFile.OpenReadStream();
            var outStream = outFile.OpenReadStream();
            ExerciseData exerciseData = new ExerciseData();

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
        [Route("exerciseId")]
        public IActionResult Get(Guid exerciseId)
        {
            return Json(context.TestData.Where(P => P.ExerciseId == exerciseId).ToList());
        }
    }
}