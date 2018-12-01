using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApp.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models.Responces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Exercises")]
    [Authorize(Roles = "User")]
    public class ExercisesController : AuthorizeController
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext applicationDbContext;

        public ExercisesController(
            ApplicationDbContext applicationDbContext,
            IMapper mapper,
            UserManager<User> userManager) : base(userManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.mapper = mapper;
        }

        [HttpPut]
        [Route("{exerciseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(Guid exerciseId, [FromBody] ExercisesViewModel model)
        {
            var exe = await applicationDbContext.Exercises.FindAsync(exerciseId);

            if (exe == null)
            {
                return NotFound();
            }

            if (model.ExerciseName != null)
            {
                exe.ExerciseName = model.ExerciseName;
            }

            if (model.ExerciseTask != null)
            {
                exe.ExerciseTask = model.ExerciseTask;
            }

            if (model.Score != -1)
            {
                exe.Score = model.Score;
            }

            await applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("{exerciseId}")]
        public IActionResult Get(Guid exerciseId)
        {
            var ex = applicationDbContext.Exercises.FirstOrDefault(P => P.ExerciseID == exerciseId);
            if (ex == null)
            {
                return NotFound();
            }
            var exView = mapper.Map<ExerciseInfo>(ex);
            var solutions = applicationDbContext
                .Solutions
                .Where(S => S.ExerciseId == exView.Id)
                .Where(S => S.UserId == UserId);
            exView.Solutions = solutions;
            return Json(exView);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(
                applicationDbContext
                .Exercises
                .Select(e => new ExerciseListResponse
                {
                    Id = e.ExerciseID,
                    Name = e.ExerciseName,
                    Score = e.Score,
                    Status = (SolutionStatus)e.Solution
                        .Where(s => s.UserId == UserId)
                        .Select(s => (int)s.Status)
                        .DefaultIfEmpty(-1)
                        .Max()

                }));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void Post([FromBody] ExercisesViewModel model)
        {
            var exeIdentity = mapper.Map<Exercise>(model);
            applicationDbContext.Exercises.Add(exeIdentity);
            applicationDbContext.SaveChanges();
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(IFormFile markdown, Guid id) 
        {
            System.Console.WriteLine(markdown.Length);
            var target = applicationDbContext.Exercises.FirstOrDefault(e => e.ExerciseID == id);
            if (target == null) return NotFound();
            using (var reader = new StreamReader(markdown.OpenReadStream()))
            {
                target.ExerciseTask = reader.ReadToEnd();
            }
            applicationDbContext.SaveChanges();
            return Ok();
        }
    }
}