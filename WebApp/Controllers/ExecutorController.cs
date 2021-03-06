using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Solutions;
using PublicAPI.Requests;
using Olympiad.Shared.Models;
using WebApp.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;
using WebApp.Models.HubModels;
using WebApp.Services;
using Models.Checking;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Executor")]
    [Authorize(Policy = "Executor")]
    public class ExecutorController : AuthorizeController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ExecutorController(
            UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IMapper mapper) : base(userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Solution>> GetAsync(Guid solutionId)
        {
            var targetSolution = await dbContext
                .Solutions
                .Where(s => s.Id == solutionId)
                .SingleOrDefaultAsync();
            if (targetSolution == null)
                return NotFound();
            return targetSolution;
        }

        [HttpPost("{solutionId}/{state}")]
        public async Task<IActionResult> Post(
            Guid solutionId, 
            SolutionStatus state,
            [FromServices] NotifyUsersService notifyUserService)
        {
            var solution = await dbContext.Solutions.SingleOrDefaultAsync(S => S.Id == solutionId);
            if (solution == null)
                return NotFound();

            solution.Status = state;
            if (state == SolutionStatus.InProcessing)
                solution.StartCheckingTime = DateTimeOffset.UtcNow;
            else
                solution.CheckedTime = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await notifyUserService.NewSolutionAdded(solution);
            return Ok();
        }

        [HttpPost("buildlog/{solutionId}")]
        public async Task<IActionResult> BuildLog(
            [FromRoute] Guid solutionId,
            [FromBody] BuildLogRequest log)
        {
            var solution = await dbContext.Solutions.SingleOrDefaultAsync(s => s.Id == solutionId);
            if (solution == null)
                return NotFound();
            var buildLogRecord = new SolutionBuildLog
            {
                BuildedTime = DateTimeOffset.UtcNow,
                Log = log.RawBuildLog,
                PrettyLog = log.PrettyBuildLog,
                SolutionId = solutionId,
                Solution = solution
            };
            dbContext.SolutionBuildLogs.Add(buildLogRecord);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("checklog/{solutionId}/{testDataId}")]
        public async Task<IActionResult> CheckLog(
            [FromRoute] Guid solutionId,
            [FromRoute] Guid testDataId,
            [FromBody] SolutionCheckRequest request)
        {
            var solution = await dbContext.Solutions.SingleOrDefaultAsync(s => s.Id == solutionId);
            if (solution == null)
                return NotFound("solution not found");

            var testData = await dbContext.TestData
                .Where(td => td.Id == testDataId)
                .Where(td => td.ExerciseDataGroup.ExerciseId == solution.ExerciseId)
                .SingleOrDefaultAsync();
            if (testData == null)
                return NotFound("exercise data not found");

            var newCheck = mapper.Map<SolutionCheck>(request);

            newCheck.SolutionId = solution.Id;
            newCheck.TestDataId = testData.Id;
            newCheck.CheckedTime = DateTimeOffset.UtcNow;

            dbContext.SolutionChecks.Add(newCheck);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}