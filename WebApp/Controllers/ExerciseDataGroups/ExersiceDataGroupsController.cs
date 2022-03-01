using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Exercises;
using PublicAPI.Requests.Exercises;
using PublicAPI.Responses;
using PublicAPI.Responses.ExercisesTestData;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
        private readonly ILogger<ExersiceDataGroupsController> logger;

        public ExersiceDataGroupsController(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<ExersiceDataGroupsController> logger,
            UserManager<User> userManager) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateTestGroups(Guid exerciseId, CreateTestDataGroupRequest createRequest)
        {
            var targetExerciseExists = await context
                .Exercises
                .AnyAsync(e => e.ExerciseID == exerciseId);
            if (!targetExerciseExists)
            {
                return NotFound("Exercise not found");
            }

            if (await context.TestDataGroups.Where(tg => tg.ExerciseId == exerciseId).AnyAsync(g => g.Title == createRequest.Title))
            {
                return Conflict("Group name already exists");
            }
            var newGroup = mapper.Map<ExerciseDataGroup>(createRequest);
            newGroup.ExerciseId = exerciseId;

            context.TestDataGroups.Add(newGroup);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Can't save data group");
                throw;
            }
            return Ok();
        }

        [HttpDelete("{groupDataId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTestGroups(Guid exerciseId, Guid groupDataId)
        {
            var targetGroup = await context.TestDataGroups
                .Where(group => group.ExerciseId == exerciseId)
                .Where(group => group.Id == groupDataId)
                .SingleOrDefaultAsync();
            
            if (targetGroup is null)
            {
                return NotFound("Exercise group not found");
            }
            
            context.TestDataGroups.Remove(targetGroup);
            await context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
