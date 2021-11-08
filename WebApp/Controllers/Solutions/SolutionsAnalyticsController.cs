using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Olympiad.Shared.Models;
using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.Solutions.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers.Solutions
{
    [Route("api/solutions/analytics")]
    [Authorize(Roles = "Admin,ResultsViewer")]
    [ApiController]
    public class SolutionsAnalyticsController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public SolutionsAnalyticsController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IMapper mapper) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("forExercise/{exerciseId:guid}")]
        public async Task<List<SolutionAnalyticCompactResponse>> GetSolutionsForExerciseAsync(Guid exerciseId, Guid userId)
        {
            var solutions = await context.Solutions
                .Where(s => s.UserId == userId)
                .Where(s => s.ExerciseId == exerciseId)
                .OrderByDescending(s => s.Status)
                .ThenByDescending(s => s.SendingTime)
                .ProjectTo<SolutionAnalyticCompactResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return solutions;
        }

        [HttpGet("{solutionId}")]
        public async Task<ActionResult<SolutionAnalyticsResponse>> GetInfoAboutSolution(Guid solutionId)
        {
            var solutionResponse = await context
                .Solutions
                .Where(s => s.Id == solutionId)
                .ProjectTo<SolutionAnalyticsResponse>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            if (solutionResponse is null)
            {
                return NotFound("Solution not found");
            }
            return Ok(solutionResponse);
        }

        [HttpGet("{solutionId:guid}/buildLogs")]
        public async Task<List<BuildLogAnalyticsResponse>> GetBuildLogs(Guid solutionId)
        {
            var buildLogs = await context
                .SolutionBuildLogs
                .Where(l => l.SolutionId == solutionId)
                .OrderByDescending(l => l.BuildedTime)
                .ProjectTo<BuildLogAnalyticsResponse>(mapper.ConfigurationProvider)
                .ToListAsync();

            return buildLogs;
        }

        [HttpGet("{solutionId:guid}/testGroupResults")]
        public async Task<List<SolutionTestGroupResulResponse>> GetTestGroupResults(Guid solutionId)
        {
            var allChecks = await context
                .SolutionChecks
                .Where(s => s.SolutionId == solutionId)
                .Select(s => new
                {
                    s.TestData.ExerciseDataGroup.Id,
                    s.TestData.ExerciseDataGroup.Title,
                    s.TestData.ExerciseDataGroup.IsPublic,
                    s.TestData.ExerciseDataGroup.Score,
                    IsSuccess = s.Status == SolutionStatus.Successful ? 1 : 0,
                    s.Status
                })
                .GroupBy(s => new
                {
                    s.Id,
                    s.Title,
                    s.IsPublic,
                    s.Score
                })
                .OrderByDescending(g => g.Key.IsPublic)
                    .ThenBy(g => g.Key.Title)
                .Select(g => new SolutionTestGroupResulResponse
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    IsPublic = g.Key.IsPublic,
                    GroupScore = g.Key.Score,
                    BestStatus = g.Max(s => s.Status)
                })
                .ToListAsync();
            return allChecks;
        }
        [HttpGet("{solutionId:guid}/checksForDataGroup")]
        public async Task<List<SolutionCheckResponse>> GetChecksForDataGroup(Guid solutionId, Guid testDataGroupId)
        {
            return await context.SolutionChecks
                .Where(sch => sch.SolutionId == solutionId)
                .Where(sch => sch.TestData.ExerciseDataGroupId == testDataGroupId)
                .ProjectTo<SolutionCheckResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
