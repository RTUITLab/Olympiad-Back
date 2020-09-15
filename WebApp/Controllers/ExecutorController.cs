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

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Executor")]
    [Authorize(Roles = "Executor")]
    public class ExecutorController : AuthorizeController
    {
        private readonly IQueueChecker queue;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ExecutorController(
            UserManager<User> userManager,
            IQueueChecker queue,
            ApplicationDbContext dbContext,
            IMapper mapper) : base(userManager)
        {
            this.queue = queue;
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
            [FromServices] IHubContext<SolutionStatusHub, IHubClient> solutionStatusHubContext)
        {
            var solution = dbContext.Solutions.FirstOrDefault(S => S.Id == solutionId);
            if (solution == null)
                return NotFound();

            solution.Status = state;
            if (state == SolutionStatus.InProcessing)
                solution.StartCheckingTime = DateTime.UtcNow;
            else
                solution.CheckedTime = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            await solutionStatusHubContext.Clients.All.UpdateSolutionStatus(new UpdateSolutionStatusModel { SolutionId = solutionId, SolutionStatus = state });
            return Ok();
        }

        [HttpPost("buildlog/{solutionId}")]
        public async Task<IActionResult> CheckLog(
            [FromRoute]Guid solutionId,
            [FromBody] string log)
        {
            var solution = dbContext.Solutions.FirstOrDefault(s => s.Id == solutionId);
            if (solution == null)
                return NotFound();
            var buildLogRecord = new SolutionBuildLog
            {
                BuildedTime = DateTime.UtcNow,
                Log = log,
                SolutionId = solutionId,
                Solution = solution
            };
            dbContext.SolutionBuildLogs.Add(buildLogRecord);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("checklog/{solutionId}")]
        public async Task<IActionResult> CheckLog(
            [FromRoute]Guid solutionId,
            [FromBody] SolutionCheckRequest request)
        {
            var solution = dbContext.Solutions.FirstOrDefault(s => s.Id == solutionId);
            if (solution == null)
                return NotFound();
            var newCheck = mapper.Map<SolutionCheck>(request);
            newCheck.SolutionId = solution.Id;
            newCheck.CheckedTime = DateTime.UtcNow;
            dbContext.SolutionChecks.Add(newCheck);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}