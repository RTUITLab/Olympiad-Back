using Executor.Executers.Build;
using Executor.Executers.Run;
using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Models.Exercises;
using Models.Solutions;
using Olympiad.Shared.Models;

namespace Executor.Executers
{
    class ExecuteWorker
    {
        private readonly Func<Guid, Task<ExerciseData[]>> getTests;
        private readonly ProgramBuilder builder;
        private readonly ProgramRunner runner;
        private readonly ILogger<ExecuteWorker> logger;


        public string Lang { get; }

        public ExecuteWorker(
            BuildProperty buildProperty,
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, string, Task> saveBuildLogs,
            Func<Guid, Task<ExerciseData[]>> getTests,
            ISolutionsBase solutionsBase,
            IDockerClient dockerClient,
            ILoggerFactory logger)
        {
            this.getTests = getTests;
            builder = new ProgramBuilder(processSolution, saveBuildLogs, BuildFinished, buildProperty, dockerClient, logger.CreateLogger<ProgramBuilder>());
            runner = new ProgramRunner(solutionsBase, dockerClient, logger.CreateLogger<ProgramRunner>());
            this.logger = logger.CreateLogger<ExecuteWorker>();
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
