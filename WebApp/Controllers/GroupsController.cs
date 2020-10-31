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
using WebApp.Services;

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

        [HttpGet("{groupId:guid}")]
        public async Task<ActionResult<GroupCompactResponse>> Get(Guid groupId)
        {
            var model = await dbContext.Groups
                .Where(c => c.Id == groupId)
                .ProjectTo<GroupCompactResponse>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            if (model == null)
            {
                return NotFound("Group not found");
            }
            return model;
        }

        [HttpGet("{groupId:guid}/full")]
        public async Task<ActionResult<GroupResponse>> GetFull(Guid groupId)
        {
            var model = await dbContext.Groups
                .Where(c => c.Id == groupId)
                .ProjectTo<GroupResponse>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            if (model == null)
            {
                return NotFound("Group not found");
            }
            return model;
        }


        [HttpPost]
        public async Task<GroupResponse> Post(
            [FromBody] GroupCreateEditRequest request,
            [FromServices] GroupsService groupsService)
        {
            var newModel = mapper.Map<Group>(request);
            newModel.InviteToken = groupsService.GenerateInvoteToken();
            dbContext.Groups.Add(newModel);
            await dbContext.SaveChangesAsync();
            return mapper.Map<GroupResponse>(newModel);
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

        [HttpPost("{groupId:guid}/join/{token}")]
        public async Task<ActionResult<GroupCompactResponse>> Join(Guid groupId, string inviteToken)
        {
            var group = await dbContext.Groups.SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound("Group not found");
            }
            if (group.InviteToken != inviteToken)
            {
                Conflict("Invite token incorrect");
            }
            dbContext.Add(new UserToGroup { GroupId = groupId, UserId = UserId });
            await dbContext.SaveChangesAsync();
            return mapper.Map<GroupCompactResponse>(group);
        }
    }
}
