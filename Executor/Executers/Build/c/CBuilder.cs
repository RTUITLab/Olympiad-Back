using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Models;

namespace Executor.Executers.Build.C
{
    [Language("c")]
    class PythonBuilder : ProgramBuilder
    {
        public PythonBuilder(Action<Guid, SolutionStatus> proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution) : base(proccessSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Program.c";


        protected override string BuildFailedCondition => "error";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
