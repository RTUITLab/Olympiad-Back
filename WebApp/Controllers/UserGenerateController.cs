using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApp.Controllers
{
    [Route("api/UserGenerate")]
    public class UserGenerateController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public UserGenerateController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost("{challengeId:guid}")]
        public async Task<IActionResult> GenerateUsers(Guid challengeId, List<string> studentIds)
        {
            List<User> generateUsers = new List<User>();

            var challenge = dbContext.Challenges.Find(challengeId);

            foreach (var studentId in studentIds)
            {


            }



        }


    }
}