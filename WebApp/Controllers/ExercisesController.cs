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
using Olympiad.Shared.Models;
using PublicAPI.Responses;
using WebApp.Models;
using PublicAPI.Requests;
using AutoMapper.QueryableExtensions;
using PublicAPI.Responses.Users;

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

        [HttpGet("compacrResults/{exerciseId:guid}")]
        public async Task<List<CompactExerciseUserResult>> GetCompactResults(Guid exerciseId)
        {
            var simpleData = await context
                     .Solutions
                     .Where(s => s.ExerciseId == exerciseId)
                     .GroupBy(m => new { m.UserId })
                     .Select(g => new
                     {
                         UserId = g.Key.UserId,
                         Status = g.Max(s => s.Status)
                     })
                     .ToListAsync();
            var userIds = simpleData.Select(s => s.UserId).ToArray();
            var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

            List<CompactExerciseUserResult> list = new List<CompactExerciseUserResult>();
            for (int i = 0; i < simpleData.Count; i++)
            {
                var data = simpleData[i];
                list[i].Status = data.Status;
                list[i].User = mapper.Map<UserInfoResponse>(users.Single(u => u.Id == data.UserId));
                list[i].TotalSum = 100;
                list[i].UserSum = 30;
                list[i].SendedTime = DateTimeOffset.UtcNow;
            }
            //var solutions = context
            //    .Solutions
            //    .Where(s => s.ExerciseId == exerciseId)
            //    .Where(s => userIds.Contains(s.UserId))
            //    .Where(s => s.Status == )
            return list;

        }

        [HttpGet]
        public async Task<List<ExerciseCompactResponse>> GetForChallenge(Guid challengeId)
        {
            var exercises = await AvailableExercises()
                .Where(e => e.ChallengeId == challengeId)
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseCompactInternalModel>(mapper.ConfigurationProvider, new { userId = UserId })
                .ToListAsync();
            return exercises.Select(e => mapper.Map<ExerciseCompactResponse>(e)).ToList();
        }

        [HttpGet]
        public async Task<List<TeacherExerciseCompactResponse>> GetForChallengeForTeacher(Guid challengeId)
        {
            var exercises = await AvailableExercises()
                .Where(e => e.ChallengeId == challengeId)
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseCompactInternalModel>(mapper.ConfigurationProvider, new { userId = UserId })
                .ToListAsync();
            return exercises.Select(e => mapper.Map<TeacherExerciseCompactResponse>(e)).ToList();
        }

        [HttpPut]
        [Route("{exerciseId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(Guid exerciseId, [FromBody] ExerciseRequest model)
        {
            var exe = await context.Exercises
                .Include(e => e.UserToExercises)
                .SingleOrDefaultAsync(e => e.ExerciseID == exerciseId);

            if (exe == null)
            {
                return NotFound("Exercise not found");
            }
            mapper.Map(model, exe);

            context.RemoveRange(exe.UserToExercises);
            if (model.SpecificUsers != null)
            {
                exe.UserToExercises
                   = model.SpecificUsers.Select(u => new UserToExercise { UserId = u, ExerciseId = exerciseId }).ToList();
            }

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("{exerciseId}")]
        public async Task<ExerciseInfo> Get(Guid exerciseId)
        {
            var exercise = await AvailableExercises()
                .Where(ex => ex.ExerciseID == exerciseId)
                .SingleOrDefaultAsync() ?? throw StatusCodeException.NotFount;

            var exView = mapper.Map<ExerciseInfo>(exercise);
            return exView;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExerciseInfo>> Post([FromBody] ExerciseExtendedRequest model)
        {
            if (!await context.Challenges.AnyAsync(c => c.Id == model.ChallengeId))
            {
                return NotFound("Challenge not found");
            }
            var exeIdentity = mapper.Map<Exercise>(model);
            if (model.SpecificUsers != null)
            {
                exeIdentity.UserToExercises
                    = model.SpecificUsers.Select(u => new UserToExercise { UserId = u, Exercise = exeIdentity }).ToList();
            }
            if (model.InOutData != null)
            {
                exeIdentity.ExerciseDatas
                    = model.InOutData.Select(u => new ExerciseData { InData = u.InData, OutData = u.OutData, IsPublic = u.IsPublic }).ToList();
            }
            context.Exercises.Add(exeIdentity);
            await context.SaveChangesAsync();
            return mapper.Map<ExerciseInfo>(exeIdentity);
        }

        [HttpPost("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(IFormFile markdown, Guid id)
        {
            var target = context.Exercises.FirstOrDefault(e => e.ExerciseID == id);
            if (target == null) return NotFound();
            using (var reader = new StreamReader(markdown.OpenReadStream()))
            {
                target.ExerciseTask = reader.ReadToEnd();
            }
            context.SaveChanges();
            return Ok();
        }

        private IQueryable<Exercise> AvailableExercises()
        {
            return context
                .Exercises
                .Where(e => e.Challenge.ChallengeAccessType == ChallengeAccessType.Public ||
                           e.Challenge.Group.UserToGroups.Any(utg => utg.UserId == UserId))
                .Where(e => e.Challenge.StartTime == null || e.Challenge.StartTime <= Now)
                .Where(e => e.UserToExercises.Count == 0 || e.UserToExercises.Any(ute => ute.UserId == UserId));
        }
    }
}