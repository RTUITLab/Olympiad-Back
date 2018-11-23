using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Executor.Executers.Build.C
{
    [Language("c")]
    class PythonBuilder : ProgramBuilder
    {
        public PythonBuilder(Func<Guid, SolutionStatus, Task> processSolution, Func<DirectoryInfo, Solution, Task> finishBuildSolution) : base(processSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Program.c";


        protected override string BuildFailedCondition => "error";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
