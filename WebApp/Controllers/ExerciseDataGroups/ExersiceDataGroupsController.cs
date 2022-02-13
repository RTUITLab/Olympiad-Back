using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using PublicAPI.Responses;
using PublicAPI.Responses.ExercisesTestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers.ExerciseDataGroups
{
    [Produces("application/json")]
    [Route("api/exercises/{exerciseId:guid}/testGroups")]
    public class ExersiceDataGroupsController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ExersiceDataGroupsController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<User> userManager) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<List<ExercisesTestDataGroupResponse>> GetTestGroups(Guid exerciseId)
        {
            var testGroups = await context
                .TestDataGroups
                .Where(g => g.ExerciseId == exerciseId)
                .OrderByDescending(g => g.IsPublic)
                .ProjectTo<ExercisesTestDataGroupResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
            return testGroups;
        }
    }
}
