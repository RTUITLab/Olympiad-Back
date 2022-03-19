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
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses.Challenges.Analytics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<string>> GetChallengeParticipants(Guid challengeId, [MaxLength(100)] string match)
        {
            return await SelectSolutionsByUserMatch(match, 
                                context.Solutions.Where(s => s.Exercise.ChallengeId == challengeId))
                .Select(s => s.User.StudentID)
                .Distinct()
                .ToListAsync();
        }

        [HttpGet("{challengeId:guid}")]
        public async Task<ListResponseWithMatch<UserChallengeResultsResponse>> GetUserResultsForChallenge(
            Guid challengeId,
            [MaxLength(100)] string match,
            [Range(0, int.MaxValue)] int offset = 0,
            [Range(1, 200)] int limit = 50)
        {
            using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);

            var targetSolutionsForChallengeQuery = context
                .Solutions
                .Where(s => s.Exercise.ChallengeId == challengeId);
            var targetSolutionsByMatchUserQuery = 
                SelectSolutionsByUserMatch(match, targetSolutionsForChallengeQuery);

            var targetUsersInfoQuery = targetSolutionsByMatchUserQuery
                .GroupBy(s => new UserChallengeResultsResponse.UserView
                {
                    Id = s.UserId,
                    StudentId = s.User.StudentID,
                    FirstName = s.User.FirstName
                });
            var totalCount = await targetUsersInfoQuery.CountAsync();

            var targetUsersInfo = await targetUsersInfoQuery
                .OrderBy(u => u.Key.StudentId)
                .Skip(offset)
                .Take(limit)
                .Select(s => s.Key)
                .ToDictionaryAsync(u => u.Id);

            var byExerciseInfo = (await targetSolutionsForChallengeQuery
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
                Limit = limit,
                Total = totalCount,
                Offset = offset,
                Match = match,
                Data = data
            };
        }

        private static IQueryable<Solution> SelectSolutionsByUserMatch(string match, IQueryable<Solution> solutionsSource)
        {
            if (string.IsNullOrWhiteSpace(match))
            {
                return solutionsSource;
            }
            var words = match.ToUpper().Split(' ');
            var targetSolutionsByMatchUserQuery = words.Aggregate(solutionsSource, (solutions, matcher) => solutions.Where(
                u =>
                    u.User.FirstName.ToUpper().Contains(matcher) ||
                    u.User.Email.ToUpper().Contains(matcher) ||
                    u.User.StudentID.ToUpper().Contains(matcher)));
            return targetSolutionsByMatchUserQuery;
        }
    }
}
