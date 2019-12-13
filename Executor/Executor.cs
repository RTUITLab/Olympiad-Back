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
        private readonly Dictionary<string, ExecuteWorker> executeWorkers;

        private readonly Dictionary<string, BuildProperty> buildProperties = new Dictionary<string, BuildProperty>
        {
            { "c", new ContainsInLogsProperty { ProgramFileName = "Program.c", BuildFailedCondition = "error" } },
            { "cpp", new ContainsInLogsProperty { ProgramFileName = "Program.cpp", BuildFailedCondition = "error" } },
            { "csharp", new ContainsInLogsProperty { ProgramFileName = "Program.cs", BuildFailedCondition = "Build FAILED" } },
            { "java", new ContainsInLogsProperty { ProgramFileName = "Main.java", BuildFailedCondition = "error" } },
            { "pasabc", new ContainsInLogsProperty { ProgramFileName = "Program.pas", BuildFailedCondition = "Compile errors:" } },
            { "python", new ContainsInLogsProperty { ProgramFileName = "Program.py", BuildFailedCondition = "error" } }
        };


        public Executor(ISolutionsBase solutionBase, IDockerClient dockerClient, ILoggerFactory logger)
        {
            executeWorkers = buildProperties.ToDictionary(
                kvp => kvp.Key,
                kvp => new ExecuteWorker(
                        kvp.Value,
                        solutionBase.SaveChanges,
                        solutionBase.GetExerciseData,
                        solutionBase,
                        dockerClient,
                        logger)
            );
            this.solutionBase = solutionBase;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                (await solutionBase.GetInQueueSolutions())
                    .ForEach(s => executeWorkers[s.Language].Handle(s));
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                Console.WriteLine("end sleep");
            }
        }
    }
}
