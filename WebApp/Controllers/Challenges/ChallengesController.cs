using System;
using System.Collections.Generic;
using System.Data;
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
using Olympiad.Shared.Models;
using PublicAPI.Requests;
using PublicAPI.Requests.Challenges;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using PublicAPI.Responses.Users;
using WebApp.Models;


namespace WebApp.Controllers.Challenges
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


        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public Task<List<ChallengeResponse>> GetAllAsync()
        {
            return context
                .Challenges
                .ProjectTo<ChallengeResponse>(mapper.ConfigurationProvider)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ChallengeResponse> Get(Guid id)
        {
            return await AvailableChallenges()
                        .Where(c => c.Id == id)
                        .SingleOrDefaultAsync() ?? throw StatusCodeException.NotFount;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<Guid> CreateDefaultChallengeAsync()
        {
            var newChallenge = new Challenge
            {
                Name = "NEW CHALLENGE NAME",
                ChallengeAccessType = ChallengeAccessType.Private,
                ViewMode = ChallengeViewMode.Hidden,
                CreationTime = DateTime.UtcNow
            };
            context.Challenges.Add(newChallenge);
            await context.SaveChangesAsync();
            return newChallenge.Id;
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChallengeResponse>> UpdateChallengeAsync(Guid id, UpdateChallengeInfoRequest request)
        {
            var targetChallenge = await context.Challenges.SingleOrDefaultAsync(c => c.Id == id);
            if (targetChallenge == null)
            {
                return NotFound("challenge not found");
            }
            mapper.Map(request, targetChallenge);
            await context.SaveChangesAsync();
            return mapper.Map<ChallengeResponse>(targetChallenge);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var targetChallenge = await context.Challenges.SingleOrDefaultAsync(c => c.Id == id);
            if (targetChallenge == null)
            {
                return NotFound("challenge not found");
            }
            context.Challenges.Remove(targetChallenge);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{challengeId:guid}/invitations")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ListResponse<UserInfoResponse>>> GetInvitations(
            [FromRoute] Guid challengeId,
            [FromQuery] ListQueryParams listQueryParams)
        {
            using var transaction = context.Database.BeginTransaction(IsolationLevel.RepeatableRead);
            var queryAll = context.Challenges
                .Where(c => c.Id == challengeId)
                .SelectMany(c => c.UsersToChallenges)
                .Select(u => u.User)
                .ProjectTo<UserInfoResponse>(mapper.ConfigurationProvider);
            var total = await queryAll.CountAsync();
            var data = await queryAll
                .OrderBy(u => u.FirstName)
                .Skip(listQueryParams.Offset)
                .Take(listQueryParams.Limit)
                .ToListAsync();
            return new ListResponse<UserInfoResponse>
            {
                Data = data,
                Limit = listQueryParams.Limit,
                Total = total,
                Offset = listQueryParams.Offset
            };
        }

        private IQueryable<ChallengeResponse> AvailableChallenges()
        {
            IQueryable<Challenge> query = context.Challenges;
            if (!IsAdmin)
            {
                query = query.Where(c =>
                    c.ChallengeAccessType == ChallengeAccessType.Public ||
                    c.UsersToChallenges.Any(utc => utc.UserId == UserId)
                );
            }

            return query.ProjectTo<ChallengeResponse>(mapper.ConfigurationProvider);
        }
    }
}
