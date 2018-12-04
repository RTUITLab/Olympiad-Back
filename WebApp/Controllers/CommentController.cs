using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApp.Models.Responses;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Comment")]
    [Authorize(Roles = "User")]
    public class CommentController : AuthorizeController
    {
        private readonly ApplicationDbContext context;

        public CommentController(UserManager<User> userManager,
            ApplicationDbContext context) : base(userManager)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostComment()
        {
            string reviewBody;

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                reviewBody = await reader.ReadToEndAsync();
            }

            Comment comment = new Comment()
            {
                Raw = reviewBody,
                UserId = UserId,
                Time = DateTime.Now
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("{pageNum}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetComments(int pageNum)
        {
            return Json(context
                .Comments
                .Skip((pageNum - 1) * 10)
                .Take(10)
                .Select(C => new CommentResponce
                {
                    UserId = UserId,
                    UserName = context.Users.FirstOrDefault(P => P.Id == C.UserId).FirstName,
                    Raw = C.Raw,
                    Time = C.Time
                }));
        }
    }
}