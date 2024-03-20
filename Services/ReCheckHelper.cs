using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Solutions;
using Olympiad.Services.SolutionCheckQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Services
{
    public class ReCheckHelper
    {
        /// <summary>
        /// Recheck solytions
        /// </summary>
        /// <param name="dbContext">Db context with all information</param>
        /// <param name="queueChecker">Queue service</param>
        /// <param name="solutionsSelector">Selector for target solutions</param>
        /// <param name="logger">Additional info logger</param>
        /// <returns>Count of solutions to recheck</returns>
        public static async Task<int> ReCheckSolutions(
            ApplicationDbContext dbContext, 
            IQueueChecker queueChecker,
            Func<ApplicationDbContext, IQueryable<Solution>> solutionsSelector,
            Func<string, Task> logger)
        {
            var solutionViews = await solutionsSelector(dbContext)
                .AsSingleQuery()
                .Select(s => new { s.Id, checks = s.SolutionChecks.Select(ch => ch.Id), logs = s.SolutionBuildLogs.Select(bl => bl.Id) })
                .ToListAsync();
            await logger($"Loaded {solutionViews.Count} ids");
            var solutions = new List<Solution>();
            foreach (var solutionView in solutionViews)
            {
                var solution = new Solution
                {
                    Id = solutionView.Id,
                    Status = Olympiad.Shared.Models.SolutionStatus.InQueue,
                    TotalScore = null
                };
                solutions.Add(solution);
                dbContext.Solutions.Attach(solution);
                dbContext.Entry(solution).Property(s => s.Status).IsModified = true;
                dbContext.Entry(solution).Property(s => s.TotalScore).IsModified = true;
                dbContext.SolutionBuildLogs.RemoveRange(solutionView.logs.Select(l => new Models.Checking.SolutionBuildLog { Id = l }));
                dbContext.SolutionChecks.RemoveRange(solutionView.checks.Select(c => new Models.Checking.SolutionCheck { Id = c }));
            }
            var saved = await dbContext.SaveChangesAsync();
            foreach (var solution in solutions)
            {
                dbContext.Entry(solution).State = EntityState.Detached;
                queueChecker.PutInQueue(solution.Id);
            }
            return solutionViews.Count;
        }
    }
}
