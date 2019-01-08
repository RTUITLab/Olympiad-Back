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
using Models.Links;
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
        public async Task<IEnumerable<ChallengeCompactResponse>> GetAsync()
        {
            return await context
                .Challenges
                .Where(c => c.ChallengeAccessType == Shared.Models.ChallengeAccessType.Public ||
                            c.UserToChallenges.Any(utc => utc.UserId == UserId))
                .ProjectTo<ChallengeCompactResponse>().ToListAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ChallengeResponse> Get(Guid id)
        {
            var query = context
                .Challenges
                .Where(c => c.Id == id);
            IQueryable<ChallengeResponse> resultQuery;
            if (!await UserManager.IsInRoleAsync(await CurrentUser(), "Admin"))
            {
                resultQuery = query
                    .Where(c => c.ChallengeAccessType == Shared.Models.ChallengeAccessType.Public ||
                           c.UserToChallenges.Any(utc => utc.UserId == UserId))
                    .ProjectTo<ChallengeResponse>();
            }
            else
            {
                resultQuery = query.ProjectTo<ChallengeExtendedResponse>();
            }
            return await resultQuery.SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ChallengeResponse> PostAsync([FromBody]ChallengeCreateRequest request)
        {
            var challenge = mapper.Map<Challenge>(request);
            challenge.CreationTime = DateTime.UtcNow;
            context.Challenges.Add(challenge);
            await context.SaveChangesAsync();
            return mapper.Map<ChallengeResponse>(challenge);
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ChallengeExtendedResponse> PutAsync(Guid id, [FromBody]ChallengeEditRequest request)
        {
            if (request.RemovePersons != null && 
                request.AddPersons != null &&
                request.RemovePersons.Intersect(request.AddPersons).Any())
                throw StatusCodeException.BadRequest;

            var targetChallenge = await context
                .Challenges
                .Include(c => c.UserToChallenges)
                .Where(c => c.Id == id)
                .SingleOrDefaultAsync()
                ?? throw StatusCodeException.NotFount;
            mapper.Map(request, targetChallenge);
            targetChallenge.UserToChallenges.RemoveAll(u => request.RemovePersons.Contains(u.UserId));
            targetChallenge.UserToChallenges.AddRange(request.AddPersons.Select(pi => new UserToChallenge
            {
                UserId = pi,
                ChallengeId = id
            }));
            await context.SaveChangesAsync();
            return mapper.Map<ChallengeExtendedResponse>(targetChallenge);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
