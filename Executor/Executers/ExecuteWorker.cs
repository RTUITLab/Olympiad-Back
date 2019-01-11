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
            BuildProperty buildProperty,
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, Task<ExerciseData[]>> getTests,
            IDockerClient dockerClient)
        {
            logger = Logger<ExecuteWorker>.CreateLogger();
            this.getTests = getTests;
            builder = new ProgramBuilder(processSolution, BuildFinished, buildProperty, dockerClient);
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
