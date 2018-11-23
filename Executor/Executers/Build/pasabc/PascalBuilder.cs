using Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Executor.Executers.Build.PascalABC
{
    [Language("pasabc")]
    class CBuilder : ProgramBuilder
    {
        public CBuilder(Func<Guid, SolutionStatus, Task> processSolution, Func<DirectoryInfo, Solution, Task> finishBuildSolution) : base(processSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Program.pas";


        protected override string BuildFailedCondition => "Compile errors:";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
