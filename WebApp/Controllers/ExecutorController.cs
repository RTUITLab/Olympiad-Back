using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Solutions;
using Shared.Models;
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

        public ExecutorController(UserManager<User> userManager,
            IQueueChecker queue,
            ApplicationDbContext dbContext) : base(userManager)
        {
            this.queue = queue;
            this.dbContext = dbContext;
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
        [Route("{exerciseId}/{state}")]
        public async Task<IActionResult> Post(Guid exerciseId, SolutionStatus state)
        {
            var solution = dbContext.Solutions.FirstOrDefault(S => S.Id == exerciseId);
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
    }
}