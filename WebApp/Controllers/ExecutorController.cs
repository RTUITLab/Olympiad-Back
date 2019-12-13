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
        public async Task<IEnumerable<Solution>> GetAsync()
        {
            var list = queue.GetFromQueue(10);
            var targetSolutions = await dbContext
                .Solutions
                .Where(s => list.Contains(s.Id))
                .ToListAsync();
            return targetSolutions;
        }

        [HttpPost()]
        [Route("{solutionId}/{state}")]
        public async Task<IActionResult> Post(Guid solutionId, SolutionStatus state)
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
            return Ok();
        }

        [HttpPost]
        [Route("checklog/{solutionId}")]
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