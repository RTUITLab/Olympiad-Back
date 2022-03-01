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
using Microsoft.Extensions.Options;
using PublicAPI.Requests;
using PublicAPI.Responses.ExercisesTestData;

namespace Executor.Executers
{
    class ExecuteWorker
    {
        private readonly Func<Guid, Task<ExerciseDataResponse[]>> getTests;
        public readonly ProgramBuilder builder;
        public readonly ProgramRunner runner;
        private readonly ILogger<ExecuteWorker> logger;

        public Solution Current { get; private set; }
        public ExecuteWorkerStatus Status { get; private set; }
        public ExecuteWorker(
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, BuildLogRequest, Task> saveBuildLogs,
            Func<Guid, Task<ExerciseDataResponse[]>> getTests,
            ISolutionsBase solutionsBase,
            IDockerClient dockerClient,
            RunningSettings runningSettings,
            StartSettings options,
            ILoggerFactory logger)
        {
            this.getTests = getTests;
            builder = new ProgramBuilder(processSolution, saveBuildLogs, dockerClient, options, logger.CreateLogger<ProgramBuilder>());
            runner = new ProgramRunner(solutionsBase, dockerClient, runningSettings, logger.CreateLogger<ProgramRunner>());
            this.logger = logger.CreateLogger<ExecuteWorker>();
        }

        public async Task Handle(Solution solution)
        {
            Current = solution;
            Status = ExecuteWorkerStatus.Build;
            var success = await builder.BuildSolution(solution);
            if (success)
            {
                var tasksForExercise = await getTests(solution.ExerciseId);
                Status = ExecuteWorkerStatus.Checking;
                await runner.RunAndCheckSolution(solution.Id, tasksForExercise);
            }
            Current = null;
            Status = ExecuteWorkerStatus.Wait;
        }
    }
}
