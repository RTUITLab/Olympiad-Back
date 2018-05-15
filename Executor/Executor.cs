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
                    (GUID, STATUS) => dbManager.SaveChanges(GUID, STATUS),
                    I => dbManager.GetExerciseData(I)
                    ))
                .ToDictionary(E => E.Lang);
            this.dbManager = dbManager;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                dbManager.GetInQueueSolutions()
                    .ForEach(S => executeWorkers[S.Language].Handle(S));
                await Task.Delay(TimeSpan.FromSeconds(10));
                Console.WriteLine("end sleep");
            }
        }
    }
}
