using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Services.SolutionCheckQueue
{
    public class DebugMockQueueChecker : IQueueChecker
    {
        private readonly ILogger<DebugMockQueueChecker> logger;

        public DebugMockQueueChecker(ILogger<DebugMockQueueChecker> logger)
        {
            this.logger = logger;
            logger.LogWarning("Using {ServiceType}", typeof(DebugMockQueueChecker).Name);
        }
        public void Clear()
        {
            logger.LogWarning("no clean in debug mode without queue");
        }

        public void PutInQueue(Guid solutionId)
        {
            logger.LogWarning("no put to queue action in debug mode without queue");
        }
    }
}
