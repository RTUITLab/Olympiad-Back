using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Services.Interfaces;

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
            while (true)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var queue = scope.ServiceProvider.GetRequiredService<IQueueChecker>();
                    var oldTime = DateTime.UtcNow - TimeSpan.FromMinutes(2);
                    var targetSolutions = await dbContext
                        .Solutions
                        .Where(s => s.Status == SolutionStatus.InProcessing)
                        .Where(s => s.StartCheckingTime < oldTime || s.StartCheckingTime == null)
                        .ToListAsync();
                    if (!targetSolutions.Any())
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30));
                        continue;
                    }
                    logger.LogInformation($"find {targetSolutions.Count} old solutions, reset");
                    targetSolutions.ForEach(s =>
                    {
                        s.Status = SolutionStatus.InQueue;
                        s.StartCheckingTime = null;
                        queue.PutInQueue(s.Id);
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
