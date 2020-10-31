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

        [HttpGet("{courseId:guid}")]
        public async Task<ActionResult<CourseResponse>> Get(Guid courseId)
        {
            var model = await dbContext.Courses
                .Where(c => c.Id == courseId)
                .ProjectTo<CourseResponse>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            if (model == null)
            {
                return NotFound("Course not found");
            }
            return model;
        }

        [HttpPost]
        public async Task<CourseCompactResponse> Post(CourseCreateRequest request)
        {
            var newModel = mapper.Map<Course>(request);
            dbContext.Courses.Add(newModel);
            await dbContext.SaveChangesAsync();
            return mapper.Map<CourseCompactResponse>(newModel);
        }

        [HttpPut("{courseId:guid}/addgroup/{groupId:guid}")]
        public async Task<ActionResult<CourseResponse>> AddGroup(Guid courseId, Guid groupId)
        {
            var course = await dbContext.Courses.Include(c => c.GroupToCourses).SingleOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                return NotFound($"Course not found");
            }
            var group = await dbContext.Groups.SingleOrDefaultAsync(c => c.Id == groupId);
            if (group == null)
            {
                return NotFound($"Group not found");
            }
            if (course.GroupToCourses.Any(gtc => gtc.GroupId == groupId))
            {
                return await Get(courseId);
            }
            dbContext.Add(new GroupToCourse { CourseId = courseId, GroupId = groupId });
            await dbContext.SaveChangesAsync();
            return await Get(courseId);
        }

        [HttpPut("{courseId:guid}/removegroup/{groupId:guid}")]
        public async Task<ActionResult<CourseResponse>> RemoveGroup(Guid courseId, Guid groupId)
        {
            var course = await dbContext.Courses.Include(c => c.GroupToCourses).SingleOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                return NotFound($"Course not found");
            }
            var group = await dbContext.Groups.SingleOrDefaultAsync(c => c.Id == groupId);
            if (group == null)
            {
                return NotFound($"Group not found");
            }
            var targetPair = course.GroupToCourses.SingleOrDefault(gtc => gtc.GroupId == groupId);
            if (targetPair == null)
            {
                return await Get(courseId);
            }
            dbContext.Remove(targetPair);
            await dbContext.SaveChangesAsync();
            return await Get(courseId);
        }

        [HttpDelete("{courseId:guid}")]
        public async Task<ActionResult<Guid>> Delete(Guid courseId)
        {
            var model = await dbContext.Courses.SingleOrDefaultAsync(c => c.Id == courseId);
            if (model == null)
                return NotFound();
            dbContext.Courses.Remove(model);
            await dbContext.SaveChangesAsync();
            return courseId;
        }
    }
}
