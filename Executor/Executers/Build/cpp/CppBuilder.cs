using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Models;

namespace Executor.Executers.Build.Cpp
{
    [Language("cpp")]
    class CppBuilder : ProgramBuilder
    {
        public CppBuilder(Action proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution) : base(proccessSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Program.cpp";

        protected override string BuildFailedCondition => "error";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
