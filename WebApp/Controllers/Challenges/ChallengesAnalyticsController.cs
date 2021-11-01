using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
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

        public ChallengesAnalyticsController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IMapper mapper) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
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

        // TODO: Currect work with limit and offset, use users count instead solutions count
        [HttpGet("{challengeId:guid}")]
        public async Task<ListResponse<UserChallengeResultsResponse>> GetUserResultsForChallenge(
            Guid challengeId,
            [MaxLength(100)] string match,
            [Range(0, int.MaxValue)] int offset = 0,
            [Range(1, 200)] int limit = 50)
        {
            var words = (match ?? "").ToUpper().Split(' ');

            var targetSolutions = context
                .Solutions
                .Where(s => s.Exercise.ChallengeId == challengeId);
            targetSolutions = words.Aggregate(targetSolutions, (solutions, matcher) => solutions.Where(
                u =>
                    u.User.FirstName.ToUpper().Contains(matcher) ||
                    u.User.Email.ToUpper().Contains(matcher) ||
                    u.User.StudentID.ToUpper().Contains(matcher)));

            var simpleDataRequest = targetSolutions
                .GroupBy(m => new
                {
                    m.UserId,
                    m.User.StudentID,
                    m.User.FirstName,
                    m.ExerciseId
                })
                .Select(g => new
                {
                    g.Key.UserId,
                    g.Key.StudentID,
                    g.Key.FirstName,
                    g.Key.ExerciseId,
                    Score = g.Max(s => s.TotalScore)
                });
            var totalCount = await simpleDataRequest.CountAsync();

            var simpleData = await simpleDataRequest
                .OrderBy(s => s.StudentID)
                .ToListAsync();
            var userSolutions = simpleData
                .GroupBy(s => new { s.UserId, s.StudentID, s.FirstName })
                .Select(g => new UserChallengeResultsResponse
                {
                    User = new UserChallengeResultsResponse.UserView
                    {
                        Id = g.Key.UserId,
                        StudentId = g.Key.StudentID,
                        FirstName = g.Key.FirstName
                    },
                    Scores = g.ToDictionary(g => g.ExerciseId.ToString(), g => g.Score),
                    TotalScore = g.Sum(e => e.Score)
                })
                .OrderBy(r => r.User.StudentId)
                .ToList();
            return new ListResponse<UserChallengeResultsResponse> { Limit = limit, Total = totalCount, Offset = offset, Data = userSolutions };
        }
    }
}
