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
using Microsoft.EntityFrameworkCore;
using Models;
using PublicAPI.Requests;
using PublicAPI.Responses;

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
        public async Task<IActionResult> PostComment(List<PostComment> comments)
        {
            var dbComments = comments.Select(c => new Comment
            {
                AuthorId = UserId,
                Raw = c.Raw,
                SendTime = DateTimeOffset.UtcNow,
                SolutionId = c.SolutionId
            }).ToList();

            context.Comments.AddRange(dbComments);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<Comment>>> GetComments(Guid solutionId)
        {
            return await context
                .Comments
                .Where(c => c.SolutionId == solutionId)
                .ToListAsync();
        }
    }
}