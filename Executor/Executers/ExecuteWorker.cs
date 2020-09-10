using Executor.Executers.Build;
using Executor.Executers.Run;
using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Models.Exercises;
using Models.Solutions;
using Olympiad.Shared.Models;
using Executor.Models.Settings;

namespace Executor.Executers
{
    class ExecuteWorker
    {
        private readonly Func<Guid, Task<ExerciseData[]>> getTests;
        public readonly ProgramBuilder builder;
        public readonly ProgramRunner runner;
        private readonly ILogger<ExecuteWorker> logger;


        public ExecuteWorker(
            BuildProperty buildProperty,
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, string, Task> saveBuildLogs,
            Func<Guid, Task<ExerciseData[]>> getTests,
            ISolutionsBase solutionsBase,
            IDockerClient dockerClient,
            RunningSettings runningSettings,
            ILoggerFactory logger)
        {
            this.getTests = getTests;
            builder = new ProgramBuilder(processSolution, saveBuildLogs, buildProperty, dockerClient, logger.CreateLogger<ProgramBuilder>());
            runner = new ProgramRunner(solutionsBase, dockerClient, runningSettings, logger.CreateLogger<ProgramRunner>());
            this.logger = logger.CreateLogger<ExecuteWorker>();
        }

        public async Task Handle(Solution solution)
        {
            var success = await builder.BuildSolution(solution);
            if (success)
            {
                var tasksForExercise = await getTests(solution.ExerciseId);
                await runner.RunAndCheckSolution(solution.Id, tasksForExercise);
            }
        }
    }
}
