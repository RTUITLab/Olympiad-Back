using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Solutions;
using PublicAPI.Requests;
using PublicAPI.Requests.Account;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses.Challenges.Analytics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Formatting;
using WebApp.Services;

namespace WebApp.Controllers.Challenges
{
    [Route("api/challenges/analytics")]
    [Authorize(Roles = "Admin,ResultsViewer")]
    [ApiController]
    public class ChallengesAnalyticsController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<ChallengesAnalyticsController> logger;

        public ChallengesAnalyticsController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<ChallengesAnalyticsController> logger) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public Task<List<ChallengeResponseWithAnalytics>> GetChallengesWithAnalitycsAsync()
        {
            return context
                .Challenges
                .ProjectTo<ChallengeResponseWithAnalytics>(mapper.ConfigurationProvider)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        [HttpGet("{challengeId:guid}/info")]
        public async Task<ChallengeResponseWithAnalytics> GetAnalyticChallengeInfo(Guid challengeId)
        {
            return await context.Challenges
                        .ProjectTo<ChallengeResponseWithAnalytics>(mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(c => c.Id == challengeId);
        }

        [HttpGet("{challengeId:guid}/participants")]
        public async Task<List<string>> GetChallengeParticipants(
            Guid challengeId,
            [MaxLength(100)] string match,
            [FromQuery, ModelBinder(typeof(ClaimRequestBinder))] IEnumerable<ClaimRequest> targetClaims)
        {
            return await context.Users
                .Where(u => u.Solutions.Any(s => s.Exercise.ChallengeId == challengeId))
                .FindByMatch(match)
                .FindByClaims(targetClaims)
                .Select(u => u.StudentID)
                .ToListAsync();
        }

        [HttpGet("{challengeId:guid}")]
        public async Task<ListResponseWithMatch<UserChallengeResultsResponse>> GetUserResultsForChallenge(
            Guid challengeId,
            [MaxLength(100)] string match,
            [FromQuery, ModelBinder(typeof(ClaimRequestBinder))] IEnumerable<ClaimRequest> targetClaims,
            [FromQuery] ListQueryParams listQueryParams)
        {
            using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);

            var targetUsersInfoQuery = context.Users
                .Where(u => u.Solutions.Any(s => s.Exercise.ChallengeId == challengeId))
                .FindByMatch(match)
                .FindByClaims(targetClaims);
            var totalCount = await targetUsersInfoQuery.CountAsync();

            var targetUsersInfo = await targetUsersInfoQuery
                .OrderBy(u => u.StudentID)
                .Skip(listQueryParams.Offset)
                .Take(listQueryParams.Limit)
                .Select(u => new UserChallengeResultsResponse.UserView
                {
                    Id = u.Id,
                    StudentId = u.StudentID,
                    FirstName = u.FirstName
                })
                .ToDictionaryAsync(u => u.Id);

            var byExerciseInfo = (await context
                .Solutions
                .Where(s => s.Exercise.ChallengeId == challengeId)
                .Where(s => targetUsersInfo.Keys.Contains(s.UserId))
                .GroupBy(s => new { s.ExerciseId, s.UserId })
                .Select(g => new { g.Key.UserId, g.Key.ExerciseId, Score = g.Max(s => s.TotalScore) })
                .ToArrayAsync())
                .GroupBy(ue => ue.UserId)
                .ToDictionary(r => r.Key);
            var data =
                targetUsersInfo.Select(u => (userInfo: u.Value, exercises: byExerciseInfo[u.Key]))
                .Select(g => new UserChallengeResultsResponse
                {
                    User = g.userInfo,
                    Scores = g.exercises.ToDictionary(g => g.ExerciseId.ToString(), g => g.Score),
                    TotalScore = g.exercises.Sum(s => s.Score)
                })
                .ToList();
            return new ListResponseWithMatch<UserChallengeResultsResponse>
            {
                Limit = listQueryParams.Limit,
                Total = totalCount,
                Offset = listQueryParams.Offset,
                Match = match,
                Data = data
            };
        }
    }
}
