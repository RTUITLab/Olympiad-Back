using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
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
        public IActionResult Get()
        {
            var list = queue.GetFromQueue(10);
            return Json(dbContext.Solutions.Where(S => list.Contains(S.Id)));
        }
    }
}