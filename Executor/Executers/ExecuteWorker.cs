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
using Models.Exercises;
using Models.Solutions;
using Shared.Models;

namespace Executor.Executers
{
    class ExecuteWorker
    {
        private readonly Func<Guid, Task<ExerciseData[]>> getTests;
        private readonly ProgramBuilder builder;
        private readonly ProgramRunner runner;
        private readonly Logger<ExecuteWorker> logger;

        public string Lang { get; }

        public ExecuteWorker(
            string lang,
            Type builderType,
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, Task<ExerciseData[]>> getTests,
            IDockerClient dockerClient)
        {
            Lang = lang;
            logger = Logger<ExecuteWorker>.CreateLogger(lang);
            this.getTests = getTests;
            Func<Solution, Task> act = BuildFinished;
            builder = Activator.CreateInstance(builderType, processSolution, act, dockerClient) as ProgramBuilder;
            runner = new ProgramRunner(processSolution, dockerClient);
        }

        public void Handle(Solution solution)
        {
            builder.Add(solution);
        }

        private async Task BuildFinished(Solution solution)
        {
            logger.LogInformation($"Finish build solution {solution.Id}, run it");
            runner.Add(solution.Id, await getTests(solution.ExerciseId));
        }
    }
}
