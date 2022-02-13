using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using PublicAPI.Responses.ExerciseTestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers.Exercises
{
    [Route("api/exercises/analytics")]
    [Authorize(Roles = "Admin,ResultsViewer")]
    [ApiController]
    public class ExercisesAnalyticsController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ExercisesAnalyticsController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IMapper mapper) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet("withAttempt")]
        public async Task<List<ExerciseCompactResponse>> GetExercisesWithAtteptsForUserAsync(
            Guid challengeId,
            Guid userId)
        {
            var exercises = await context.Exercises
                .Where(e => e.ChallengeId == challengeId)
                .Where(e => e.Solutions.Any(s => s.UserId == userId))
                .OrderBy(e => e.ExerciseName)
                .ProjectTo<ExerciseCompactResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return exercises;
        }
    }
}
