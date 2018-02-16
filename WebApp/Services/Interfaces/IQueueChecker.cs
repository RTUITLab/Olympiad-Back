using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services.Interfaces
{
    public interface IQueueChecker
    {
        void PutInQueue(Guid solutionId);
        List<Guid> GetFromQueue(int count);
    }
}
