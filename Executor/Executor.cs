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

namespace Executor
{
    class Executor
    {
        private ApplicationDbContext db;
        private readonly Dictionary<string, ExecuteWorker> executeWorkers;
        private object connectionString;

        public Executor(string dbConnectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(dbConnectionString);
            db = new ApplicationDbContext(optionsBuilder.Options);
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
                    () => db.SaveChanges(),
                    I => db.TestData
                        .Where(D => D.ExerciseId == I)
                        .ToArray()
                    ))
                .ToDictionary(E => E.Lang);
        }

        public void Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                db
                .Solutions
                .Where(S => S.Status == SolutionStatus.InQueue)
                .ToList()
                .ForEach(S => executeWorkers[S.Language].Handle(S));
                Thread.Sleep(TimeSpan.FromSeconds(10));
                Console.WriteLine("end sleep");
            }
        }
    }
}
