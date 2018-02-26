using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Executor.Executers.Run
{
    abstract class ProgramRunner
    {
        protected readonly ConcurrentQueue<(Solution solution, ExerciseData[] testData, DirectoryInfo binaries)> solutionsQueue
            = new ConcurrentQueue<(Solution, ExerciseData[], DirectoryInfo)>();
        private readonly Action<Guid, SolutionStatus> proccessSolution;


        private Task runningTask;
        private SemaphoreSlim runningSemaphore;

        public ProgramRunner(Action<Guid, SolutionStatus> proccessSolution)
        {
            runningSemaphore = new SemaphoreSlim(0, 1);
            runningTask = Task.Run(RunLoop);
            this.proccessSolution = proccessSolution;
        }


        public void Add(Solution solution, ExerciseData[] datas, DirectoryInfo binaries)
        {
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
            proccessSolution(task.solution.Id, result);
        }

        protected SolutionStatus Run(DirectoryInfo binaries, ExerciseData testData)
        {
            File.WriteAllText(Path.Combine(binaries.FullName, "in.txt"), testData.InData);
            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"run --rm -v {binaries.FullName}:/src runner:{Language}"
                },
            };

            proccess.OutputDataReceived += (E, A) => { };
            proccess.ErrorDataReceived += (E, A) => { };
            var success = proccess.Start();
            proccess.BeginErrorReadLine();
            proccess.BeginOutputReadLine();
            Console.WriteLine($"Started bool {success}");
            proccess.WaitForExit();

            var status = CheckSolution(binaries.FullName, testData.OutData);
            binaries.Delete(true);

            return status;
        }

        private SolutionStatus CheckSolution(string filesPath, string correctOut)
        {
            var errorData = File.ReadAllText(Path.Combine(filesPath, "err.txt")).TrimEnd();
            if (errorData != string.Empty)
                return SolutionStatus.RunTimeError;

            var outData = File.ReadAllText(Path.Combine(filesPath, "out.txt")).TrimEnd();
            if (outData == correctOut.TrimEnd())
                return SolutionStatus.Sucessful;

            return SolutionStatus.WrongAnswer;
        }
        private string lang;
        private string Language
        {
            get
            {
                return lang ??
                    (lang = GetType().GetCustomAttribute<LanguageAttribute>().Lang);
            }
        }
    }
}