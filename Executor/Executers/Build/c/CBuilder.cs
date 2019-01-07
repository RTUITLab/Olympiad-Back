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

namespace Executor.Executers.Build.C
{
    [Language("c")]
    class PythonBuilder : ProgramBuilder
    {
        public PythonBuilder(Func<Guid, SolutionStatus, Task> processSolution, Func<DirectoryInfo, Solution, Task> finishBuildSolution, IDockerClient dockerClient) : base(processSolution, finishBuildSolution, dockerClient)
        {
        }

        protected override string ProgramFileName => "Program.c";


        protected override string BuildFailedCondition => "error";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }
}
