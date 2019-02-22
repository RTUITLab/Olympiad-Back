using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models;
using PublicAPI.Responses;
using WebApp.Models.Settings;

namespace WebApp.Controllers
{
    [Route("api/UserGenerate")]
    public class UserGenerateController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly IOptions<GenerateSettings> options;

        public UserGenerateController(ApplicationDbContext dbContext,
            UserManager<User> userManager,
            IOptions<GenerateSettings> options)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.options = options;
        }
        [HttpPost]
        public async Task<IActionResult> GenerateUsers([FromBody]List<string> studentIds)
        {
            const int MIN = 11111111;
            const int MAX = 99999999;

            List<GenerateUsersRespponce> generates = new List<GenerateUsersRespponce>();


            foreach (var studentId in studentIds)
            {
                User user = new User()
                {
                    UserName = $"{studentId}{options.Value.Domain}",
                    StudentID = studentId
                };
                var password = new Random().Next(MIN, MAX).ToString();

                generates.Add(new GenerateUsersRespponce { Email = user.UserName, Password = password });
                var result = await userManager.CreateAsync(user, password);
                result = await userManager.AddToRoleAsync(user, "User");
            }
            return Json(generates);
        }
    }
}