using Microsoft.EntityFrameworkCore;
using Models;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Services.UserSolutionsReport
{
    public class UserSolutionsReportCreator
    {
        private readonly static UserSolutionsReportOptions defaultOptions = new UserSolutionsReportOptions
        {
            ShowChecks = true,
            ShowName = true,
            SolutionsMode = ShowSolutionsMode.OnlyBest
        };
        private readonly ApplicationDbContext dbContext;

        public UserSolutionsReportCreator(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> CreateMarkdownReport(string userStudentId, Guid challengeId, UserSolutionsReportOptions options = null)
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.StudentID == userStudentId)
                ?? throw new ArgumentException($"Not found user {userStudentId}");
            var challenge = await dbContext.Challenges.SingleOrDefaultAsync(c => c.Id == challengeId)
                ?? throw new ArgumentException($"Not found challeng {challengeId}");
            options ??= defaultOptions;

            var builder = new StringBuilder();
            builder.AppendLine($"## Report about **{challenge.Name}** for **{(options.ShowName ? user.FirstName : user.StudentID)}**");

            builder.AppendLine($"### Additional info");
            builder.AppendLine($"Field | Value");
            builder.AppendLine($"- | -");
            builder.AppendLine($"Student ID | {user.StudentID}");
            builder.AppendLine($"User ID | {user.Id}");
            builder.AppendLine($"Challenge ID | {challenge.Id}");

            var exercises = await dbContext.Exercises
                .Where(e => e.ChallengeId == challenge.Id)
                .Where(e => e.Solutions.Any(s => s.UserId == user.Id))
                .OrderBy(e => e.ExerciseName)
                .ToListAsync();

            var exerciseBlocks = new List<(SolutionStatus status, string view)>();

            foreach (var exercise in exercises)
            {
                builder.AppendLine(await BuildExercise(user, exercise, options));
            }

            return builder.ToString();
        }

        private async Task<string> BuildExercise(User user, Models.Exercises.Exercise exercise, UserSolutionsReportOptions options)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"## {exercise.ExerciseName}");
            builder.AppendLine(exercise.ExerciseTask);
            builder.AppendLine();

            var solutionsForExercise = await LoadSolutionsForExercise(exercise.ExerciseID, user.Id);
            switch (options.SolutionsMode)
            {
                case ShowSolutionsMode.AllByDescendingStatus:
                    foreach (var (solution, i) in solutionsForExercise.Select((s, i) => (s, i + 1)))
                    {
                        builder.AppendLine($"### Solution #{i}");
                        RenderSolution(builder, solution, options.ShowChecks);
                    }
                    break;
                case ShowSolutionsMode.OnlyBest:
                    builder.AppendLine($"### Best solution");
                    RenderSolution(builder, solutionsForExercise[0], options.ShowChecks);
                    break;
            };

            builder.AppendLine("---");
            builder.AppendLine();
            return builder.ToString();
        }

        private static void RenderSolution(StringBuilder builder, Models.Solutions.Solution solution, bool showChecks)
        {
            builder.AppendLine($"Field|Value");
            builder.AppendLine($"-|-");
            builder.AppendLine($"Lang|{solution.Language}");
            builder.AppendLine($"Status|{solution.Status}");
            builder.AppendLine($"ID|{solution.Id}");
            builder.AppendLine($"Sent|{solution.SendingTime}");

            builder.AppendLine($"```{solution.Language}");
            builder.AppendLine(solution.Raw);
            builder.AppendLine($"```");
            if (solution.Status != SolutionStatus.Successful && showChecks)
            {
                foreach (var (check, num) in solution.SolutionChecks
                    .Where(ch => ch.Status != SolutionStatus.Successful)
                    .Select((c, i) => (c, i)))
                {
                    builder.AppendLine($"#### Check {num + 1} ({check.Status})");

                    builder.AppendLine($"#### Example IN");
                    builder.AppendLine($"```");
                    builder.AppendLine(TrimProgramInOut(check.ExampleIn.Replace(' ', '·')));
                    builder.AppendLine($"```");

                    builder.AppendLine($"#### Example OUT");
                    builder.AppendLine($"```");
                    builder.AppendLine(TrimProgramInOut(check.ExampleOut.Replace(' ', '·')));
                    builder.AppendLine($"```");

                    builder.AppendLine($"#### Program OUT");
                    builder.AppendLine($"```");
                    builder.AppendLine(TrimProgramInOut(check.ProgramOut));
                    builder.AppendLine($"```");

                    if (!string.IsNullOrEmpty(check.ProgramErr))
                    {
                        builder.AppendLine($"#### Program ERR");
                        builder.AppendLine($"```");
                        builder.AppendLine(TrimProgramInOut(check.ProgramErr));
                        builder.AppendLine($"```");
                    }
                }
            }
        }

        private static string TrimProgramInOut(string data)
        {
            if (data == null)
            {
                return data;
            }
            if (data.Length > 250)
            {
                data = data.Substring(0, 250);
                data += "\n !!Trimmed, too big for report!!";
            }
            return data.Replace(' ', '·');
        }

        private async Task<List<Models.Solutions.Solution>> LoadSolutionsForExercise(Guid exerciseId, Guid userId)
        {
            return await dbContext
                .Solutions
                .Include(s => s.SolutionBuildLogs)
                .Include(s => s.SolutionChecks)
                .OrderByDescending(s => s.Status)
                .ThenByDescending(s => s.SendingTime)
                .Where(s => s.UserId == userId && s.ExerciseId == exerciseId)
                .ToListAsync();
        }
    }

}
