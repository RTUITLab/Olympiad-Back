using System;
using System.Collections.Generic;

namespace WebApp.Services.Interfaces
{
    public interface IQueueChecker
    {
        void Clear();
        void PutInQueue(Guid solutionId);
        List<Guid> GetFromQueue(int count);
    }
}
