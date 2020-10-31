using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Lessons;
using PublicAPI.Requests.Lessons;
using PublicAPI.Responses.Lessons;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : AuthorizeController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public CoursesController(
            UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IMapper mapper
            ) : base(userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public Task<List<CourseCompactResponse>> Get()
        {
            return dbContext.Courses
                .ProjectTo<CourseCompactResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<CourseCompactResponse> Post(CourseCreateRequest request)
        {
            var newModel = mapper.Map<Course>(request);
            dbContext.Courses.Add(newModel);
            await dbContext.SaveChangesAsync();
            return mapper.Map<CourseCompactResponse>(newModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> Delete(Guid id)
        {
            var model = await dbContext.Courses.SingleOrDefaultAsync(c => c.Id == id);
            if (model == null)
                return NotFound();
            dbContext.Courses.Remove(model);
            await dbContext.SaveChangesAsync();
            return id;
        }
    }
}
