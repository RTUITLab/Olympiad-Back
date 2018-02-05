using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Executor.Executers.Build
{
    abstract class ProgramBuilder
    {
        protected readonly ConcurrentQueue<Solution> solutionsQueue = new ConcurrentQueue<Solution>();
        private readonly Action proccessSolution;
        private readonly Action<DirectoryInfo, Solution> finishBuildSolution;

        protected abstract string Lang { get; }
        protected abstract DirectoryInfo Build(Solution solution);


        private Task buildingTask;
        private SemaphoreSlim buildingSemaphore;
        public ProgramBuilder(Action proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution)
        {
            buildingSemaphore = new SemaphoreSlim(0, 1);
            buildingTask = Task.Run(BuildLoop);
            this.proccessSolution = proccessSolution;
            this.finishBuildSolution = finishBuildSolution;
        }
        public void Add(Solution solution)
        {
            if (solution.Language != Lang) throw new ArgumentException("solution language not supported");
            if (solutionsQueue.Any(S => S.Id == solution.Id)) return;
            solutionsQueue.Enqueue(solution);
            buildingSemaphore.Release();
        }
        private async Task BuildLoop()
        {
            while (true)
            {
                await buildingSemaphore.WaitAsync();
                while(solutionsQueue.TryDequeue(out var solution))
                {
                    solution.Status = SolutionStatus.InProcessing;
                    proccessSolution();
                    var result = Build(solution);
                    finishBuildSolution(result, solution);
                }
            }
        }
    }
}
