using Executor.Executers.Build;
using Executor.Executers.Run;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Executor.Logging;
namespace Executor.Executers
{
    class ExecuteWorker
    {
        private readonly string lang;
        private readonly Func<Guid, ExerciseData[]> getTests;
        private ProgramBuilder builder;
        private ProgramRunner runner;
        private Logger<ExecuteWorker> logger;

        public string Lang => lang;

        public ExecuteWorker(
            string lang,
            Type builderType,
            Type runnerType,
            Action<Guid, SolutionStatus> proccessSolution,
            Func<Guid, ExerciseData[]> getTests)
        {
            this.lang = lang;
            logger = Logger<ExecuteWorker>.CreateLogger(lang);
            this.getTests = getTests;
            Action<DirectoryInfo, Solution> act = BuildFinished;
            builder = Activator.CreateInstance(builderType, new object[]
            {
                proccessSolution,
                act
            }) as ProgramBuilder;
            runner = Activator.CreateInstance(runnerType, new object[]
            {
                proccessSolution
            }) as ProgramRunner;
        }

        public void Handle(Solution solution)
        {
            builder.Add(solution);
        }

        private void BuildFinished(DirectoryInfo dirInfo, Solution solution)
        {
            logger.LogInformation($"Finish build solution {solution.Id}, run it");
            runner.Add(solution, getTests(solution.ExerciseId), dirInfo);
        }
    }
}
