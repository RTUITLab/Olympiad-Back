using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using Olympiad.Services.SolutionCheckQueue;
using Olympiad.Shared.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class RestartCheckingService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<RestartCheckingService> logger;

        public RestartCheckingService(
            IServiceProvider serviceProvider,
            ILogger<RestartCheckingService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var queue = scope.ServiceProvider.GetRequiredService<IQueueChecker>();
                var oldTime = DateTime.UtcNow - TimeSpan.FromMinutes(2);
                var targetSolutions = await dbContext
                    .Solutions
                    .Where(s => s.Exercise.Type == ExerciseType.Code)
                    .Where(s => s.Exercise.ExerciseDataGroups.Any())
                    .Where(s => s.Status == SolutionStatus.InProcessing)
                    .Where(s => s.StartCheckingTime < oldTime || s.StartCheckingTime == null)
                    .ToListAsync(cancellationToken: stoppingToken);
                if (targetSolutions.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }
                logger.LogInformation("Found {OldSolutionsCount} old solutions, reset", targetSolutions.Count);
                targetSolutions.ForEach(s =>
                {
                    s.Status = SolutionStatus.InQueue;
                    s.StartCheckingTime = null;
                    queue.PutInQueue(s.Id);
                });
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
