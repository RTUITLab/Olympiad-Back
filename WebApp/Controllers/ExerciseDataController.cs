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
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Exercises;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/ExerciseData")]
    public class ExerciseDataController : AuthorizeController
    {
        private readonly ApplicationDbContext context;

        public ExerciseDataController(
            ApplicationDbContext context, 
            UserManager<User> userManager) : base(userManager)
        {
            this.context = context;
        }


        [HttpPost]
        [Route("{exerciseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(Guid exerciseId, [FromBody]ExerciseDataViewModel exerciseDataIn)
        {
            if (exerciseDataIn == null || string.IsNullOrEmpty(exerciseDataIn.InData) || string.IsNullOrEmpty(exerciseDataIn.OutData))
            {
                return BadRequest("No exercise data in request");
            }

            if (!context.Exercises.Any(e => e.ExerciseID == exerciseId))
            {
                return BadRequest("No target exercise");
            }

            var exerciseData = new ExerciseData
            {
                ExerciseId = exerciseId,
                InData = exerciseDataIn.InData,
                OutData = exerciseDataIn.OutData,
                IsPublic = exerciseDataIn.IsPublic
            };
            await context.TestData.AddAsync(exerciseData);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("{exerciseId}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid exerciseId)
        {
            var targetUser = await UserManager.GetUserAsync(User);
            var dbRequest = context
                .TestData
                .Where(p => p.ExerciseId == exerciseId);

            if (!await UserManager.IsInRoleAsync(targetUser, "Admin") &&
                !await UserManager.IsInRoleAsync(targetUser, "Executor"))
                dbRequest = dbRequest
                    .Where(p => p.IsPublic);
            return Json(
                await dbRequest
                .ToListAsync());
        }

        [HttpPut]
        [Route("{exerciseDataId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(Guid exerciseDataId, [FromBody]ExerciseDataViewModel exerciseDataIn)
        {
            if (exerciseDataIn == null || string.IsNullOrEmpty(exerciseDataIn.InData) || string.IsNullOrEmpty(exerciseDataIn.OutData))
            {
                return BadRequest("No exercise data in request");
            }

            var testData = await context.TestData.SingleOrDefaultAsync(e => e.Id == exerciseDataId);
            if (testData == null)
            {
                return BadRequest("No target exercise");
            }
            testData.InData = exerciseDataIn.InData;
            testData.OutData = exerciseDataIn.OutData;
            testData.IsPublic = exerciseDataIn.IsPublic;

            await context.SaveChangesAsync();

            return Ok();
        }

    }
}