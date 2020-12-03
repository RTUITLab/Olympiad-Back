using System;
using System.Collections.Generic;

namespace Olympiad.Services
{
    public interface IQueueChecker
    {
        void Clear();
        void PutInQueue(Guid solutionId);
    }
}
