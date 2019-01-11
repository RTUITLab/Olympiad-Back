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

namespace Executor
{
    class Executor
    {
        private DbManager dbManager;
        private readonly Dictionary<string, ExecuteWorker> executeWorkers;

        public Executor(DbManager dbManager, IDockerClient dockerClient)
        {
            executeWorkers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType == typeof(ProgramBuilder))
                .Select(t => new { builderType = t, lang = t.GetCustomAttribute<LanguageAttribute>().Lang })
                .Select(p => new ExecuteWorker(
                    p.lang,
                    p.builderType,
                    dbManager.SaveChanges,
                    dbManager.GetExerciseData,
                    dockerClient
                    )
                )
                .ToDictionary(E => E.Lang);
            this.dbManager = dbManager;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                (await dbManager.GetInQueueSolutions())
                    .ForEach(s => executeWorkers[s.Language].Handle(s));
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                Console.WriteLine("end sleep");
            }
        }
    }
}
