using Amazon.Runtime.Internal.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Solutions;
using Olympiad.Services.SolutionCheckQueue;
using Olympiad.Shared;
using Olympiad.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services.Solutions
{
    public class SolutionsService : ISolutionsService
    {
        private readonly ApplicationDbContext context;
        private readonly IQueueChecker queueChecker;
        private readonly ILogger<SolutionsService> logger;
        private static readonly TimeSpan onePerTimePostLimit = TimeSpan.FromMinutes(1);
        public SolutionsService(ApplicationDbContext context, IQueueChecker queueChecker, ILogger<SolutionsService> logger)
        {
            this.context = context;
            this.queueChecker = queueChecker;
            this.logger = logger;
        }
        public async Task<Solution> PostSolution(string fileBody, ProgramRuntime runtime, Guid exerciseId, Guid authorId, ISolutionsService.PostSolutionChecks solutionChecks)
        {
            var now = DateTimeOffset.UtcNow;
            if (solutionChecks.HasFlag(ISolutionsService.PostSolutionChecks.ChallengeAvailable))
            {
                await CheckChallengeAvailable(exerciseId, authorId, now);
            }
            if (solutionChecks.HasFlag(ISolutionsService.PostSolutionChecks.TooManyPost))
            {
                await CheckTooManyRequests(exerciseId, authorId, now);
            }
            if (solutionChecks.HasFlag(ISolutionsService.PostSolutionChecks.AlreadySent))
            {
                await CheckAlreadySent(fileBody, exerciseId, authorId);
            }

            if (solutionChecks.HasFlag(ISolutionsService.PostSolutionChecks.ExerciseRuntimeRestrictions))
            {
                await CheckExerciseRuntime(exerciseId, runtime);
            }

            Solution solution = new Solution()
            {
                Raw = fileBody,
                Language = runtime,
                ExerciseId = exerciseId,
                UserId = authorId,
                Status = SolutionStatus.InQueue,
                SendingTime = now
            };

            await context.Solutions.AddAsync(solution);
            await context.SaveChangesAsync();
            logger.LogInformation("Created solution {SolutionId} for exercise {ExerciseId} and user {UserId}", solution.Id, exerciseId, authorId);
            if (await context.TestData.AnyAsync(td => td.ExerciseDataGroup.ExerciseId == exerciseId))
            {
                logger.LogInformation("Put solution {SolutionId} to test queue", solution.Id);
                queueChecker.PutInQueue(solution.Id);
            }
            else
            {
                logger.LogWarning("No exercise data for {SolutionId}, no put to test queue", solution.Id);
            }
            return solution;
        }

        private async Task CheckExerciseRuntime(Guid exerciseId, ProgramRuntime programRuntime)
        {
            var restrictions = await context.Exercises
                .Where(ex => ex.ExerciseID == exerciseId)
                .Select(ex => ex.Restrictions)
                .SingleAsync();
            var allowedRuntimes = restrictions?.Code?.AllowedRuntimes?.Select(ProgramRuntime.FromValue) ?? ProgramRuntime.List;
            if (!allowedRuntimes.Contains(programRuntime))
            {
                throw new ISolutionsService.ExerciseRuntimesRestrictionException(allowedRuntimes);
            }
        }

        private async Task CheckChallengeAvailable(Guid exerciseId, Guid authorId, DateTimeOffset now)
        {
            if (!await context.Exercises.AnyAsync(
                                e => e.ExerciseID == exerciseId &&
                                (e.Challenge.StartTime == null || e.Challenge.StartTime <= now) &&
                                (e.Challenge.EndTime == null || e.Challenge.EndTime >= now) &&
                                (e.Challenge.ChallengeAccessType == ChallengeAccessType.Public || e.Challenge.UsersToChallenges.Any(utc => utc.UserId == authorId))))
            {
                throw new ISolutionsService.ChallengeNotAvailableException();
            }
        }

        private async Task CheckAlreadySent(string fileBody, Guid exerciseId, Guid authorId)
        {
            var oldSolution = await context
                                .Solutions
                                .Where(s => s.UserId == authorId)
                                .Where(s => s.Raw == fileBody)
                                .Where(s => s.ExerciseId == exerciseId)
                                .FirstOrDefaultAsync();
            if (oldSolution != null)
            {
                throw new ISolutionsService.AlreadySentException();
            }
        }

        private async Task CheckTooManyRequests(Guid exerciseId, Guid authorId, DateTimeOffset now)
        {
            var lastSendingDate = (await context
                .Solutions
                .Where(s => s.UserId == authorId)
                .Where(s => s.ExerciseId == exerciseId)
                .Select(s => s.SendingTime)
                .ToListAsync())
                .DefaultIfEmpty(DateTimeOffset.MinValue)
                .Max();

            if ((now - lastSendingDate) < onePerTimePostLimit)
            {
                throw new ISolutionsService.TooManyPostException(1, onePerTimePostLimit);
            }
        }
    }
}
