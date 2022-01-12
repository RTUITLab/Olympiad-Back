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
using PublicAPI.Responses.Exercises;
using System.Linq.Expressions;

namespace WebApp.Controllers.Exercises
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
            context = applicationDbContext;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<List<ExerciseForUserInfoResponse>> GetForChallenge(Guid challengeId)
        {
            var exercises = await context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .Where(AvailableExercise(UserId))
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseCompactInternalModel>(mapper.ConfigurationProvider, new { userId = UserId })
                .ToListAsync();
            return exercises.Select(e => mapper.Map<ExerciseForUserInfoResponse>(e)).ToList();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,ResultsViewer")]
        public async Task<List<ExerciseCompactResponse>> GetAllForChallenge(Guid challengeId)
        {
            var exercises = await context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseCompactResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return exercises;
        }

        [HttpGet("all/withtests")]
        [Authorize(Roles = "Admin,ResultsViewer")]
        public async Task<List<ExerciseWithTestCasesCountResponse>> GetAllWithTestsForChallenge(Guid challengeId)
        {
            var exercises = await context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseWithTestCasesCountResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return exercises;
        }

        [HttpGet]
        [Route("all/{exerciseId}")]
        [Authorize(Roles = "Admin,ResultsViewer")]
        public async Task<ExerciseInfo> GetForAdmin(Guid exerciseId)
        {
            var exercise = await context
                .Exercises
                .Where(ex => ex.ExerciseID == exerciseId)
                .ProjectTo<ExerciseInfo>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
            return exercise;
        }

        [HttpGet]
        [Route("{exerciseId}")]
        public async Task<ExerciseInfo> Get(Guid exerciseId)
        {
            var exercise = await context
                .Exercises
                .Where(ex => ex.ExerciseID == exerciseId)
                .Where(AvailableExercise(UserId))
                .ProjectTo<ExerciseInfo>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;

            return exercise;
        }

        private Expression<Func<Exercise, bool>> AvailableExercise(Guid userId)
        {
            return ex =>
                // Can get info only for started challenges
                ex.Challenge.StartTime == null || ex.Challenge.StartTime <= Now &&
                (
                    // Can get exercise for publick challenge
                    ex.Challenge.ChallengeAccessType == ChallengeAccessType.Public ||
                    // Or for private for invite
                    ex.Challenge.UsersToChallenges.Any(utc => utc.UserId == userId)
                );

        }
    }
}