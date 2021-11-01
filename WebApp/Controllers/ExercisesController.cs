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
        public async Task<List<ExerciseForUserInfoResponse>> GetForChallenge(Guid challengeId)
        {
            var exercises = await context
                .Exercises
                .Where(e => e.ChallengeId == challengeId)
                .Where(e => e.Challenge.ChallengeAccessType == ChallengeAccessType.Public ||
                           e.Challenge.UsersToChallenges.Any(utc => utc.UserId == UserId))
                .Where(e => e.Challenge.StartTime == null || e.Challenge.StartTime <= Now)
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

        [HttpGet]
        [Route("{exerciseId}")]
        public async Task<ExerciseInfo> Get(Guid exerciseId)
        {
            var exercise = await context
                .Exercises
                .Where(ex => ex.ExerciseID == exerciseId)
                .Where(e => e.Challenge.StartTime == null || e.Challenge.StartTime <= Now)
                .ProjectTo<ExerciseInfo>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;

            return exercise;
        }
    }
}