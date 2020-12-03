using Microsoft.EntityFrameworkCore;
using Models;
using Models.Exercises;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympiad.Services
{
    /// <summary>
    /// Deprecated
    /// </summary>
    public class ChallengesService
    {
        private readonly ApplicationDbContext dbContext;

        public ChallengesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Challenge>> GetChallenges()
        {
            return await dbContext.Challenges.ToListAsync();
        }
        public async Task<Challenge> GetChallenge(Guid challengeId)
        {
            return await dbContext.Challenges.SingleOrDefaultAsync(ch => ch.Id == challengeId);
        }
    }
}
