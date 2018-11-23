using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Executor.Executers.Build.Python
{
    [Language("python")]
    class PythonBuilder : ProgramBuilder
    {
        public PythonBuilder(Func<Guid, SolutionStatus, Task> processSolution, Func<DirectoryInfo, Solution, Task> finishBuildSolution) : base(processSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Program.py";

        protected override DirectoryInfo Build(Solution solution)
        {
            var sourceDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            File.WriteAllText(Path.Combine(sourceDir.FullName, ProgramFileName), solution.Raw, new UTF8Encoding(false));
            return sourceDir;
        }
        protected override string BuildFailedCondition => "error";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
