using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.Solutions;
using Olympiad.Services;
using Olympiad.Shared.Models;
using WebApp.Models.Settings;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Configure
{
    public class FillQueue : IConfigureWork
    {
        private readonly IQueueChecker queueChecker;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<FillQueue> logger;

        public FillQueue(
            IQueueChecker queueChecker,
            ApplicationDbContext dbContext,
            ILogger<FillQueue> logger)
        {
            this.queueChecker = queueChecker;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task Configure(CancellationToken cancellationToken)
        {
            var solutionsToQueue = await dbContext
                .Solutions
                .Where(s => s.Status == SolutionStatus.InQueue)
                .Select(s => s.Id)
                .ToListAsync();
            logger.LogInformation($"Solutions to queue: {solutionsToQueue.Count}");
            for (int i = 0; i < solutionsToQueue.Count; i++)
            {
                logger.LogInformation($"{i,10} / {solutionsToQueue.Count,-10}");
                Guid solutionId = solutionsToQueue[i];
                queueChecker.PutInQueue(solutionId);
            }
        }
    }
}
