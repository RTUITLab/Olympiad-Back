using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ResultsViewer.Extensions
{
    public class LockSafe<T> : OwningComponentBase<T>
    {
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public async Task DoSafe(Func<T, Task> action)
        {
            await semaphore.WaitAsync();
            try
            {
                await action(Service);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
