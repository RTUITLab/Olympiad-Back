using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Executor.Executers.Run
{
    abstract class ProgramRunner
    {
        protected readonly ConcurrentQueue<(Solution solution, ExerciseData[] testData, DirectoryInfo binaries)> solutionsQueue
            = new ConcurrentQueue<(Solution, ExerciseData[], DirectoryInfo)>();
        private readonly Action proccessSolution;

        protected abstract string Lang { get; }
        protected abstract SolutionStatus Run(DirectoryInfo binaries, ExerciseData testData);

        private Task runningTask;
        private SemaphoreSlim runningSemaphore;

        public ProgramRunner(Action proccessSolution)
        {
            runningSemaphore = new SemaphoreSlim(0, 1);
            runningTask = Task.Run(RunLoop);
            this.proccessSolution = proccessSolution;
        }


        public void Add(Solution solution, ExerciseData[] datas, DirectoryInfo binaries)
        {
            if (solution.Language != Lang) throw new ArgumentException("solution language not supported");
            if (solutionsQueue.Any(S => S.solution.Id == solution.Id)) return;
            solutionsQueue.Enqueue((solution, datas, binaries));
            runningSemaphore.Release();
        }
        private async Task RunLoop()
        {
            while (true)
            {
                await runningSemaphore.WaitAsync();
                while (solutionsQueue.TryDequeue(out var solution))
                {
                    HandleSolution(solution);
                }
            }
        }

        private void HandleSolution((Solution solution, ExerciseData[] testData, DirectoryInfo binaries) task)
        {
            SolutionStatus result = SolutionStatus.Sucessful;
            foreach (var data in task.testData)
            {
                result = Run(task.binaries, data);
                if (result != SolutionStatus.Sucessful)
                {
                    break;
                }
            }
            task.solution.Status = result;
            proccessSolution();
        }
    }
}
