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

namespace Executor
{
    class Executor
    {
        private DbManager dbManager;
        private readonly Dictionary<string, ExecuteWorker> executeWorkers;

        public Executor(DbManager dbManager)
        {
            executeWorkers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(T =>
                    T.BaseType == typeof(ProgramBuilder) ||
                    T.BaseType == typeof(ProgramRunner))
                .GroupBy(T => T.GetCustomAttribute<LanguageAttribute>().Lang)
                .Select(G => new ExecuteWorker(
                    G.Key,
                    G.First(T => T.BaseType == typeof(ProgramBuilder)),
                    G.First(T => T.BaseType == typeof(ProgramRunner)),
                    dbManager.SaveChanges,
                    dbManager.GetExerciseData
                    ))
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
