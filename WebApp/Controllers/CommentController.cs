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
        public async Task<IActionResult> PostComment(IFormFile review)
        {
            string reviewBody;

            if (review == null)
            {
                return BadRequest("Вы пытаетесь оставить пустой комментарий");
            }

            var stream = review.OpenReadStream();

            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                reviewBody = await streamReader.ReadToEndAsync();
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
    }
}