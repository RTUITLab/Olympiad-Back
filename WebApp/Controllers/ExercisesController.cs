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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Models.Exercises;
using Models.Solutions;
using Shared.Models;
using PublicAPI.Responses;
using WebApp.Models;
using PublicAPI.Requests;
using AutoMapper.QueryableExtensions;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/exercises")]
    [Authorize(Roles = "User")]
    public class ExercisesController : AuthorizeController
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;

        public ExercisesController(
            ApplicationDbContext applicationDbContext,
            IMapper mapper,
            UserManager<User> userManager) : base(userManager)
        {
            this.context = applicationDbContext;
            this.mapper = mapper;
        }


        [HttpGet]
        public Task<List<ExerciseCompactResponse>> GetForChallenge(Guid challengeId)
        {
            return context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .Where(e => e.Challenge.ChallengeAccessType == ChallengeAccessType.Public ||
                           e.Challenge.UsersToChallenges.Any(utc => utc.UserId == UserId))
                .Where(e => e.Challenge.StartTime == null || e.Challenge.StartTime <= Now)
                .ProjectTo<ExerciseCompactResponse>()
                .ToListAsync();
        }

        [HttpPut]
        [Route("{exerciseId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(Guid exerciseId, [FromBody] ExerciseRequest model)
        {
            var exe = await context.Exercises.FindAsync(exerciseId);

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

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("{exerciseId}")]
        public async Task<ExerciseInfo> Get(Guid exerciseId)
        {
            var exercise = await context
                .Exercises
                .Where(e => e.Challenge.StartTime == null || e.Challenge.StartTime <= Now)
                .SingleOrDefaultAsync(p => p.ExerciseID == exerciseId)
                ?? throw StatusCodeException.NotFount;
            
            var solutions = await context
                .Solutions
                .Where(s => s.ExerciseId == exerciseId)
                .Where(s => s.UserId == UserId)
                .ToListAsync();
            exercise.Solutions = solutions;
            var exView = mapper.Map<ExerciseInfo>(exercise);
            return exView;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ExerciseInfo> Post([FromBody] ExerciseRequest model)
        {
            if (!await context.Challenges.AnyAsync(c => c.Id == model.ChallengeId))
                throw StatusCodeException.BadRequest();

            var exeIdentity = mapper.Map<Exercise>(model);
            context.Exercises.Add(exeIdentity);
            await context.SaveChangesAsync();
            return mapper.Map<ExerciseInfo>(exeIdentity);
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(IFormFile markdown, Guid id) 
        {
            System.Console.WriteLine(markdown.Length);
            var target = context.Exercises.FirstOrDefault(e => e.ExerciseID == id);
            if (target == null) return NotFound();
            using (var reader = new StreamReader(markdown.OpenReadStream()))
            {
                target.ExerciseTask = reader.ReadToEnd();
            }
            context.SaveChanges();
            return Ok();
        }
    }
}