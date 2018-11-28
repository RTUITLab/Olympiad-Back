using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;

namespace Executor.Executers.Build.dotnet
{
    [Language("csharp")]
    class JavaBuilder : ProgramBuilder
    {
        public JavaBuilder(Func<Guid, SolutionStatus, Task> processSolution, Func<DirectoryInfo, Solution, Task> finishBuildSolution, IDockerClient dockerClient)
            : base(processSolution, finishBuildSolution, dockerClient)
        {
        }

        protected override string ProgramFileName => "Program.cs";


        protected override string BuildFailedCondition => "Build FAILED";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => Path.Combine(startDir.FullName, "pub");
    }

}

