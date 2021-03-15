using Microsoft.EntityFrameworkCore;
using Models;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Admin.Services
{
    public class UserSolutionsReportCreator
    {
        private readonly ApplicationDbContext dbContext;

        public UserSolutionsReportCreator(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> CreateMarkdownReport(Guid userId, Guid challengeId)
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId)
                ?? throw new ArgumentException($"Not found user {userId}");
            var challenge = await dbContext.Challenges.SingleOrDefaultAsync(c => c.Id == challengeId)
                ?? throw new ArgumentException($"Not found challeng {challengeId}");

            var builder = new StringBuilder();
            builder.AppendLine($"# Report about **{challenge.Name}** for **{user.FirstName}**");

            builder.AppendLine($"## Additional info");
            builder.AppendLine($"Field|Value");
            builder.AppendLine($"-|-");
            builder.AppendLine($"Student ID|{user.StudentID}");
            builder.AppendLine($"User ID|{user.Id}");
            builder.AppendLine($"Challenge ID|{challenge.Id}");

            var exercises = await dbContext.Exercises
                .Where(e => e.ChallengeId == challenge.Id)
                .Where(e => e.Solutions.Any(s => s.UserId == user.Id))
                .OrderBy(e => e.ExerciseName)
                .ToListAsync();

            var exerciseBlocks = new List<(SolutionStatus status, string view)>();

            foreach (var exercise in exercises)
            {
                exerciseBlocks.Add(await BuildExercise(user, exercise));
            }

            foreach (var (status, view) in exerciseBlocks.OrderBy(e => e.status))
            {
                builder.AppendLine(view);
            }


            return builder.ToString();
        }

        private async Task<(SolutionStatus, string)> BuildExercise(User user, Models.Exercises.Exercise exercise)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"# {exercise.ExerciseName}");
            builder.AppendLine(exercise.ExerciseTask);
            builder.AppendLine();

            var bestSolution = await LoadSolutionsForExercise(exercise.ExerciseID, user.Id);
            builder.AppendLine($"## Best solution");
            builder.AppendLine($"Field|Value");
            builder.AppendLine($"-|-");
            builder.AppendLine($"Lang|{bestSolution.Language}");
            builder.AppendLine($"Status|{bestSolution.Status}");
            builder.AppendLine($"ID|{bestSolution.Id}");

            builder.AppendLine($"```{bestSolution.Language}");
            builder.AppendLine(bestSolution.Raw);
            builder.AppendLine($"```");
            if (bestSolution.Status != Olympiad.Shared.Models.SolutionStatus.Successful)
            {

                foreach (var (check, num) in bestSolution.SolutionChecks
                    .Where(ch => ch.Status != Olympiad.Shared.Models.SolutionStatus.Successful)
                    .Select((c, i) => (c, i)))
                {
                    builder.AppendLine($"### Check {num + 1}");

                    builder.AppendLine($"### Example IN");
                    builder.AppendLine($"```");
                    builder.AppendLine(check.ExampleIn);
                    builder.AppendLine($"```");

                    builder.AppendLine($"### Example OUT");
                    builder.AppendLine($"```");
                    builder.AppendLine(check.ExampleOut);
                    builder.AppendLine($"```");

                    builder.AppendLine($"### Program OUT");
                    builder.AppendLine($"```");
                    builder.AppendLine(check.ProgramOut);
                    builder.AppendLine($"```");

                    if (!string.IsNullOrEmpty(check.ProgramErr))
                    {
                        builder.AppendLine($"### Program ERR");
                        builder.AppendLine($"```");
                        builder.AppendLine(check.ProgramErr);
                        builder.AppendLine($"```");
                    }
                }
            }


            builder.AppendLine("---");
            builder.AppendLine();
            return (bestSolution.Status, builder.ToString());
        }

        private async Task<Models.Solutions.Solution> LoadSolutionsForExercise(Guid exerciseId, Guid userId)
        {
            return await dbContext
                .Solutions
                .Include(s => s.SolutionBuildLogs)
                .Include(s => s.SolutionChecks)
                .OrderByDescending(s => s.Status)
                .ThenByDescending(s => s.SendingTime)
                .Where(s => s.UserId == userId && s.ExerciseId == exerciseId)
                .FirstOrDefaultAsync();
        }
    }

}
