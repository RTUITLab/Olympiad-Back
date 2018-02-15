﻿using System;
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

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Exercises")]
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
                .Select(E => new ExerciseListResponse
                {
                    Id = E.ExerciseID,
                    Name = E.ExerciseName,
                    Score = E.Score,
                    Status = (SolutionStatus)E.Solution
                        .Where(S => S.UserId == UserId)
                        .Select(S => (int)S.Status)
                        .DefaultIfEmpty(-1)
                        .Max()

                }));
        }

        [HttpPost]
        public void Post([FromBody] ExercisesViewModel model)
        {
            var exeIdentity = mapper.Map<Exercise>(model);
            applicationDbContext.Exercises.Add(exeIdentity);
            applicationDbContext.SaveChanges();
        }
    }
}