using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Models;
using Models.Solutions;
using Shared.Models;

namespace Executor.Executers.Build.Python
{
    [Language("python")]
    class PythonBuilder : ProgramBuilder
    {
        public PythonBuilder(Func<Guid, SolutionStatus, Task> processSolution, Func<Solution, Task> finishBuildSolution, IDockerClient dockerClient) : base(processSolution, finishBuildSolution, dockerClient)
        {
        }

        protected override string ProgramFileName => "Program.py";

        protected override string BuildFailedCondition => "error";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
