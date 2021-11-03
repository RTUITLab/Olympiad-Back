using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
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
            IMapper mapper) : base (userManager)
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
    }
}
