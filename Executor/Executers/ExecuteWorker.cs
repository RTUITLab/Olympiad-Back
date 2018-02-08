using Executor.Executers.Build;
using Executor.Executers.Run;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Executor.Executers
{
    class ExecuteWorker
    {
        private readonly string lang;
        private readonly Func<Guid, ExerciseData[]> getTests;
        private ProgramBuilder builder;
        private ProgramRunner runner;

        public string Lang => lang;

        public ExecuteWorker(
            string lang,
            Type builderType,
            Type runnerType,
            Action proccessSolution,
            Func<Guid, ExerciseData[]> getTests)
        {
            this.lang = lang;
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
            runner.Add(solution, getTests(solution.ExerciseId), dirInfo);
        }
    }
}
