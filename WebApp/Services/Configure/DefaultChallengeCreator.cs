using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.Exercises;
using Olympiad.Shared.Models;
using RTUITLab.AspNetCore.Configure.Configure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Models.Settings;

namespace WebApp.Services.Configure
{
    public class DefaultChallengeCreator : IConfigureWork
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DefaultChallengeSettings defaultChallengeData;
        private readonly ILogger<DefaultChallengeCreator> logger;

        public DefaultChallengeCreator(
            ApplicationDbContext dbContext, 
            IOptions<DefaultChallengeSettings> options,
            ILogger<DefaultChallengeCreator> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.defaultChallengeData = options.Value;
        }

        public async Task Configure(CancellationToken cancellationToken)
        {
            var defaultChallengeExists = await dbContext.Challenges.AnyAsync(ch => ch.Name == defaultChallengeData.Title);
            if (defaultChallengeExists)
            {
                logger.LogInformation($"Challenge {defaultChallengeData.Title} already exists, skip creating");
                return;
            }
            var newChallenge = new Challenge
            {
                ChallengeAccessType = ChallengeAccessType.Public,
                CreationTime = DateTime.UtcNow,
                Name = defaultChallengeData.Title,
                Description = defaultChallengeData.Description,
                Exercises = defaultChallengeData.Exercises.Select(ex => new Exercise
                {
                    ExerciseName = ex.Title,
                    ExerciseTask = ex.Description,
                    Score = 1,
                    ExerciseDatas = ex.TestData.Select(td => new ExerciseData
                    {
                        InData = td.Input,
                        OutData = td.Output,
                        IsPublic = td.IsPublic
                    }).ToList()
                }).ToList()
            };
            dbContext.Challenges.Add(newChallenge);
            var savedCount = await dbContext.SaveChangesAsync();
            logger.LogInformation($"Created default challenge, saved {savedCount} entries, challenge id: {newChallenge.Id}");
        }
    }
}
