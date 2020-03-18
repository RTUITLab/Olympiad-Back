using Executor.Executers;
using Executor.Executers.Build;
using Executor.Executers.Run;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Executor.Models.Settings;
using Microsoft.Extensions.Options;
using System.Xml.XPath;

namespace Executor
{
    class Executor
    {
        private ISolutionsBase solutionBase;
        private readonly ILogger<Executor> logger;
        public readonly Dictionary<string, ExecuteWorker> executeWorkers;

        public int BuildQueueLength => executeWorkers.Select(w => w.Value.builder.BuildQueueLength).Sum();
        public int RunQueueLength => executeWorkers.Select(w => w.Value.runner.RunQueueLength).Sum();

        private readonly Dictionary<string, BuildProperty> buildProperties = new Dictionary<string, BuildProperty>
        {
            { "c", new ContainsInLogsProperty { ProgramFileName = "Program.c", BuildFailedCondition = "error" } },
            { "cpp", new ContainsInLogsProperty { ProgramFileName = "Program.cpp", BuildFailedCondition = "error" } },
            { "csharp", new ContainsInLogsProperty { ProgramFileName = "Program.cs", BuildFailedCondition = "Build FAILED" } },
            { "java", new ContainsInLogsProperty { ProgramFileName = "Main.java", BuildFailedCondition = "error" } },
            { "pasabc", new ContainsInLogsProperty { ProgramFileName = "Program.pas", BuildFailedCondition = "Compile errors:" } },
            { "python", new ContainsInLogsProperty { ProgramFileName = "Program.py", BuildFailedCondition = "error" } },
            { "fpas", new ContainsInLogsProperty { ProgramFileName = "Program.pas", BuildFailedCondition = "error" } },
        };


        public Executor(
            ISolutionsBase solutionBase,
            IDockerClient dockerClient,
            ILoggerFactory loggerFactory,
            IOptions<RunningSettings> runningOptions,
            ILogger<Executor> logger)
        {
            executeWorkers = buildProperties.ToDictionary(
                kvp => kvp.Key,
                kvp => new ExecuteWorker(
                        kvp.Value,
                        solutionBase.SaveChanges,
                        solutionBase.SaveBuildLog,
                        solutionBase.GetExerciseData,
                        solutionBase,
                        dockerClient,
                        runningOptions.Value,
                        loggerFactory)
            );
            this.solutionBase = solutionBase;
            this.logger = logger;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var pair in executeWorkers)
                {
                    await HandleWorker(pair.Key, pair.Value);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        private async Task HandleWorker(string lang, ExecuteWorker worker)
        {
            if ((worker.runner.Current == null || worker.runner.RunQueueLength == 0) && worker.builder.Current == null)
            {
                var solutionToCheck = await solutionBase.GetInQueueSolutions(lang, 1);
                solutionToCheck
                        .ForEach(s => worker.Handle(s));
                logger.LogInformation($"added {solutionToCheck.Count} solutions");
            }
        }
    }
}
