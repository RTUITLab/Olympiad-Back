using System;
using System.Collections.Generic;

namespace Olympiad.Services.SolutionCheckQueue
{
    public interface IQueueChecker
    {
        void Clear();
        void PutInQueue(Guid solutionId);
    }
}
