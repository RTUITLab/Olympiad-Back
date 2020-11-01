using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Exercises;
using Olympiad.Shared.Models;
using PublicAPI.Requests.Challenges;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using WebApp.Models;


namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/challenges")]
    [Authorize(Roles = "User")]
    public class ChallengesController : AuthorizeController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ChallengesController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IMapper mapper
            ) : base(userManager)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public Task<List<ChallengeResponse>> GetAsync()
        {
            return AvailableChallenges().OrderBy(c => c.Name).ToListAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ChallengeResponse> Get(Guid id)
        {
            return await AvailableChallenges()
                        .Where(c => c.Id == id)
                        .SingleOrDefaultAsync() ?? throw StatusCodeException.NotFount;
        }

        private IQueryable<ChallengeResponse> AvailableChallenges()
        {
            IQueryable<Challenge> query = context.Challenges;
            if (!IsAdmin)
            {
                query = query
                    .Where(c => c.ChallengeAccessType == ChallengeAccessType.Public ||
                           c.Group.UserToGroups.Any(utg => utg.UserId == UserId));
            }
            return query.ProjectTo<ChallengeResponse>(mapper.ConfigurationProvider);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChallengeResponse>> PostAsync([FromBody] ChallengeCreateEditRequest request)
        {
            if (request.ChallengeAccessType == ChallengeAccessType.Private && request.GroupId == null)
            {
                return BadRequest("You must specify group id for private challenge");
            }
            if (request.ChallengeAccessType == ChallengeAccessType.Public)
            {
                request.GroupId = null;
            }
            var challenge = mapper.Map<Challenge>(request);
            challenge.CreationTime = DateTimeOffset.UtcNow;
            context.Challenges.Add(challenge);
            await context.SaveChangesAsync();
            return mapper.Map<ChallengeResponse>(challenge);
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChallengeResponse>> PutAsync(Guid id, [FromBody]ChallengeCreateEditRequest request)
        {
            if (request.ChallengeAccessType == ChallengeAccessType.Private && request.GroupId == null)
            {
                return BadRequest("You must specify group id for private challenge");
            }
            if (request.ChallengeAccessType == ChallengeAccessType.Public)
            {
                request.GroupId = null;
            }

            var targetChallenge = await context
                .Challenges
                .SingleOrDefaultAsync(c => c.Id == id)
                ?? throw StatusCodeException.NotFount;

            mapper.Map(request, targetChallenge);

            await context.SaveChangesAsync();
            return mapper.Map<ChallengeResponse>(targetChallenge);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
