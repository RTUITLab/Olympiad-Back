using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using PublicAPI.Requests;
using PublicAPI.Responses;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class GroupsController : AuthorizeController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public GroupsController(
            UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IMapper mapper
            ) : base(userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public Task<List<GroupCompactResponse>> Get()
        {
            return dbContext.Groups
                .ProjectTo<GroupCompactResponse>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<GroupCompactResponse> Post(GroupCreateRequest request)
        {
            var newModel = mapper.Map<Group>(request);
            dbContext.Groups.Add(newModel);
            await dbContext.SaveChangesAsync();
            return mapper.Map<GroupCompactResponse>(newModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> Delete(Guid id)
        {
            var model = await dbContext.Groups.SingleOrDefaultAsync(c => c.Id == id);
            if (model == null)
                return NotFound();
            dbContext.Groups.Remove(model);
            await dbContext.SaveChangesAsync();
            return id;
        }
    }
}
