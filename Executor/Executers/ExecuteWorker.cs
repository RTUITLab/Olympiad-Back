using Executor.Executers.Build;
using Executor.Executers.Run;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Executor.Logging;
namespace Executor.Executers
{
    class ExecuteWorker
    {
        private readonly string lang;
        private readonly Func<Guid, Task<ExerciseData[]>> getTests;
        private readonly ProgramBuilder builder;
        private readonly ProgramRunner runner;
        private readonly Logger<ExecuteWorker> logger;

        public string Lang => lang;

        public ExecuteWorker(
            string lang,
            Type builderType,
            Type runnerType,
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, Task<ExerciseData[]>> getTests,
            IDockerClient dockerClient)
        {
            this.lang = lang;
            logger = Logger<ExecuteWorker>.CreateLogger(lang);
            this.getTests = getTests;
            Func<DirectoryInfo, Solution, Task> act = BuildFinished;
            builder = Activator.CreateInstance(builderType, processSolution, act, dockerClient) as ProgramBuilder;
            runner = Activator.CreateInstance(runnerType, processSolution) as ProgramRunner;
        }

        public void Handle(Solution solution)
        {
            builder.Add(solution);
        }

        private async Task BuildFinished(DirectoryInfo dirInfo, Solution solution)
        {
            logger.LogInformation($"Finish build solution {solution.Id}, run it");
            runner.Add(solution, await getTests(solution.ExerciseId), dirInfo);
        }
    }
}
