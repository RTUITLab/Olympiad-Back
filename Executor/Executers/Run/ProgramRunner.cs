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
using Executor.Logging;
namespace Executor.Executers.Run
{
    abstract class ProgramRunner
    {
        private readonly BlockingCollection<(Solution solution, ExerciseData[] testData, DirectoryInfo binaries)> solutionQueue
            = new BlockingCollection<(Solution solution, ExerciseData[] testData, DirectoryInfo binaries)>();
        private readonly ConcurrentQueue<(Solution solution, ExerciseData[] testData, DirectoryInfo binaries)> solutionsQueue
            = new ConcurrentQueue<(Solution, ExerciseData[], DirectoryInfo)>();
        private readonly Func<Guid, SolutionStatus, Task> processSolution;


        private Task runningTask;
        private readonly Logger<ProgramRunner> logger;
        public ProgramRunner(Func<Guid, SolutionStatus, Task> processSolution)
        {
            logger = Logger<ProgramRunner>.CreateLogger(Language);
            runningTask = Task.Run(RunLoop);
            this.processSolution = processSolution;
        }


        public void Add(Solution solution, ExerciseData[] data, DirectoryInfo binaries)
        {
            logger.LogInformation($"add solution {solution.Id}, data count: {data.Length}");
            if (solutionQueue.Any(s => s.solution.Id == solution.Id)) return;
            solutionQueue.Add((solution, data, binaries));
        }
        private async Task RunLoop()
        {
            await Task.Yield();
            foreach (var solutionPack in solutionQueue.GetConsumingEnumerable())
            {
                logger.LogInformation($"Check built solution {solutionPack.solution.Id}");
                await HandleSolution(solutionPack);
            }
        }

        private async Task HandleSolution((Solution solution, ExerciseData[] testData, DirectoryInfo binaries) task)
        {
            var result = SolutionStatus.Sucessful;
            foreach (var data in task.testData)
            {
                result = Run(task.binaries, data);
                logger.LogTrace($"check solution {task.solution.Id} in {data.InData} out {data.OutData} result: {result}");
                if (result != SolutionStatus.Sucessful)
                {
                    break;
                }
            }
            task.solution.Status = result;
            await processSolution(task.solution.Id, result);
        }

        protected SolutionStatus Run(DirectoryInfo binaries, ExerciseData testData)
        {
            File.WriteAllText(Path.Combine(binaries.FullName, "in.txt"), testData.InData);
            var process = new Process()
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

            process.OutputDataReceived += (e, a) => { };
            process.ErrorDataReceived += (e, a) => { };
            var success = process.Start();
            logger.LogTrace($"started process {process.Id} success: {success}");
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            logger.LogTrace($"process ${process.Id} finish");
            var status = CheckSolution(binaries.FullName, testData.OutData);
            binaries.Delete(true);

            return status;
        }

        private static SolutionStatus CheckSolution(string filesPath, string correctOut)
        {
            var errorData = File.ReadAllText(Path.Combine(filesPath, "err.txt")).TrimEnd();
            if (errorData != string.Empty)
                return SolutionStatus.RunTimeError;

            var outData = File.ReadAllText(Path.Combine(filesPath, "out.txt")).TrimEnd();
            return outData == correctOut.TrimEnd() ? SolutionStatus.Sucessful : SolutionStatus.WrongAnswer;
        }
        private string lang;
        private string Language => lang ??
                                   (lang = GetType().GetCustomAttribute<LanguageAttribute>().Lang);
    }
}