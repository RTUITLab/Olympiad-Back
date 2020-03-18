using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Olympiad.Shared.Models;
using Olympiad.Shared.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private readonly IOptions<AdminSettings> options;
        private readonly IQueueChecker queue;
        private readonly ApplicationDbContext db;

        public AdminController(
            IOptions<AdminSettings> options,
            IQueueChecker queue,
            ApplicationDbContext db)
        {
            this.options = options;
            this.queue = queue;
            this.db = db;
        }
        [AllowAnonymous]
        [HttpPost("forceresetqueue")]
        public async Task<ActionResult<int>> ForceResetQueue()
        {
            if (HttpContext.Request.Headers["Authorization"].ToString() != options.Value.SecurityKey)
                return NotFound();

           var targetSolutions = await db
                .Solutions
                .Where(s => s.Status == SolutionStatus.InProcessing || s.Status == SolutionStatus.InQueue)
                .ToListAsync();
            queue.Clear();
            targetSolutions.ForEach(s =>
            {
                s.Status = SolutionStatus.InQueue;
                s.StartCheckingTime = null;
                queue.PutInQueue(s.Id);
            });
            var saved = await db.SaveChangesAsync();
            return Json(saved);
        }
    }
}
