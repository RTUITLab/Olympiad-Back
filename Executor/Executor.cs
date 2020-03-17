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
                        loggerFactory)
            );
            this.solutionBase = solutionBase;
            this.logger = logger;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (BuildQueueLength + RunQueueLength > 3)
                {
                    logger.LogInformation("Too large queue of tasks, waiting");
                }
                else
                {
                    var inQueueSolutions = await solutionBase.GetInQueueSolutions();
                    inQueueSolutions
                        .ForEach(s => executeWorkers[s.Language].Handle(s));
                    logger.LogInformation($"added {inQueueSolutions.Count} solutions");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}
