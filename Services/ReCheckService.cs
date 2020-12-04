using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Services
{
    public class ReCheckService
    {
        public static async Task<int> ReCheckSolutions(
            ApplicationDbContext dbContext, 
            IQueueChecker queueChecker,
            Func<ApplicationDbContext, IQueryable<Solution>> solutionsSelector,
            Func<string, Task> logger)
        {
            var solutionViews = await solutionsSelector(dbContext)
                .Select(s => new { s.Id, checks = s.SolutionChecks.Select(ch => ch.Id), logs = s.SolutionBuildLogs.Select(bl => bl.Id) })
                .ToListAsync();
            await logger($"Loaded {solutionViews.Count} ids");
            var solutions = new List<Models.Solutions.Solution>();
            foreach (var solutionView in solutionViews)
            {
                var solution = new Models.Solutions.Solution
                {
                    Id = solutionView.Id,
                    Status = Olympiad.Shared.Models.SolutionStatus.InQueue
                };
                solutions.Add(solution);
                dbContext.Solutions.Attach(solution);
                dbContext.Entry(solution).Property(s => s.Status).IsModified = true;
                dbContext.SolutionBuildLogs.RemoveRange(solutionView.logs.Select(l => new Models.Checking.SolutionBuildLog { Id = l }));
                dbContext.SolutionChecks.RemoveRange(solutionView.checks.Select(c => new Models.Checking.SolutionCheck { Id = c }));
            }
            var saved = await dbContext.SaveChangesAsync();
            foreach (var solution in solutions)
            {
                dbContext.Entry(solution).State = EntityState.Detached;
                queueChecker.PutInQueue(solution.Id);
            }
            return saved;
        }
    }
}
