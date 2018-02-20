using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Models;

namespace Executor.Executers.Build.PascalABC
{
    [Language("pasabc")]
    class CBuilder : ProgramBuilder
    {
        public CBuilder(Action proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution) : base(proccessSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Program.pas";

        public override string DockerImageName => "builder:pasabc";

        protected override string BuildFailedCondition => "Compile errors:";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
