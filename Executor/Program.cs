using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Executor.Executers.Build;
using Executor.Executers.Build.dotnet;
using Executor.Executers.Run;
using Executor.Executers.Run.dotnet;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Executor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = JsonConvert.DeserializeObject<JObject>(await File.ReadAllTextAsync("appsettings.Secret.json"));
            var connectionString = options["ConnectionStrings"]["OlympDB"].ToString();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                Program.db = db;
                ListenSolutions(db);
            }



            Console.ReadLine();
        }
        static ProgramRunner runner = new DotnetRunner(() => SetSolutionStatus(db));
        static ApplicationDbContext db;
        static void ListenSolutions(ApplicationDbContext db)
        {
            ProgramBuilder builder = new DotnetBuilder(() => SetSolutionStatus(db), (D, S) => { HandleBuildedSolution(D, S, db); });
            while (true)
            {
                var solutions = db
                    .Solutions
                    .Where(S => S.Status == SolutionStatus.InQueue)
                    .Where(S => S.Language == "csharp")
                    .ToList();
                foreach (var solution in solutions)
                {
                    builder.Add(solution);
                }
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private static void HandleBuildedSolution(DirectoryInfo d, Solution s, ApplicationDbContext db)
        {
            var tests = db.TestData
                .Where(D => D.ExerciseId == s.ExerciseId)
                .ToArray();
            runner.Add(s, tests, d);
        }

        static void SetSolutionStatus(ApplicationDbContext db)
        {
            db.SaveChanges();
        }
    }
}
