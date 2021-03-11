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
using Olympiad.Shared.Models;
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
            IQueryable<Challenge> query = context
                .Challenges
                .Where(c => c.ChallengeAccessType == ChallengeAccessType.Public ||
                        c.UsersToChallenges.Any(utc => utc.UserId == UserId));
            return query.ProjectTo<ChallengeResponse>(mapper.ConfigurationProvider);
        }
    }
}
