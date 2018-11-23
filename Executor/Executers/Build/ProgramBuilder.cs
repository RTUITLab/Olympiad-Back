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
namespace Executor.Executers.Build
{
    abstract class ProgramBuilder
    {
        private readonly BlockingCollection<Solution> solutionQueue = new BlockingCollection<Solution>();
        private readonly Func<Guid, SolutionStatus, Task> processSolution;
        private readonly Func<DirectoryInfo, Solution, Task> finishBuildSolution;

        //protected abstract DirectoryInfo Build(Solution solution);
        protected abstract string ProgramFileName { get; }
        protected abstract string BuildFailedCondition { get; }
        protected abstract string GetBinariesDirectory(DirectoryInfo startDir);

        private Task buildingTask;
        private readonly SemaphoreSlim buildingSemaphore;
        private readonly Logger<ProgramBuilder> logger;
        public ProgramBuilder(
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<DirectoryInfo, Solution, Task> finishBuildSolution)
        {
            logger = Logger<ProgramBuilder>.CreateLogger(Language);
            buildingSemaphore = new SemaphoreSlim(0, 1);
            buildingTask = Task.Run(BuildLoop);
            this.processSolution = processSolution;
            this.finishBuildSolution = finishBuildSolution;
        }
        public void Add(Solution solution)
        {
            logger.LogDebug($"Add solution {solution.Id}");
            if (solutionQueue.Any(s => s.Id == solution.Id)) return;
            solutionQueue.Add(solution);
        }
        private async Task BuildLoop()
        {
            await Task.Yield();
            foreach (var solution in solutionQueue.GetConsumingEnumerable())
            {
                solution.Status = SolutionStatus.InProcessing;
                await processSolution(solution.Id, SolutionStatus.InProcessing);
                logger.LogInformation($"build solution {solution.Id}");
                var binsDirectory = default(DirectoryInfo);
                try
                {
                    binsDirectory = Build(solution);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Build solution {solution.Id} with error", ex);
                }
                if (solution.Status == SolutionStatus.CompileError)
                {
                    await processSolution(solution.Id, SolutionStatus.CompileError);
                    continue;
                }
                logger.LogInformation("finish build solution");
                await finishBuildSolution(binsDirectory, solution);
            }
        }

        protected virtual DirectoryInfo Build(Solution solution)
        {
            var sourceDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            logger.LogDebug($"new dir is {sourceDir.FullName}");
            File.WriteAllText(Path.Combine(sourceDir.FullName, ProgramFileName), solution.Raw, new UTF8Encoding(false));

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"run --rm -v {sourceDir.FullName}:/src/src builder:{Language}"
                },
            };

            var solStatus = SolutionStatus.InProcessing;
            process.OutputDataReceived += (d, e) => ProcessOutputDataReceived(d, e, ref solStatus);
            process.ErrorDataReceived += (e, a) => ProcessOutputDataReceived(e, a, ref solStatus);
            var success = process.Start();
            logger.LogDebug($"started process {process.Id}, success: {success}");
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            logger.LogDebug($"process exited {process.Id}");
            if (solStatus == SolutionStatus.CompileError)
            {
                solution.Status = SolutionStatus.CompileError;
                return null;
            }
            var binPath = GetBinariesDirectory(sourceDir);
            var newBinPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.Move(binPath, newBinPath);
            logger.LogDebug($"path for binaries: {newBinPath}");
            return new DirectoryInfo(newBinPath);
        }
        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e, ref SolutionStatus status)
        {
            logger.LogTrace($"out data: {e.Data}");
            if (e.Data?.Contains(BuildFailedCondition) != true) return;
            logger.LogInformation($"message {e.Data} contains {BuildFailedCondition}, generate CompileError status");
            status = SolutionStatus.CompileError;
        }
        private string lang;
        private string Language => lang ??
                                   (lang = GetType().GetCustomAttribute<LanguageAttribute>().Lang);
    }
}
