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

namespace Executor.Executers.Build.Java
{
    [Language("java")]
    class JavaBuilder : ProgramBuilder
    {
        public JavaBuilder(Func<Guid, SolutionStatus, Task> processSolution, Func<DirectoryInfo, Solution, Task> finishBuildSolution, IDockerClient dockerClient)
            : base(processSolution, finishBuildSolution, dockerClient)
        {
        }

        protected override string ProgramFileName => "Main.java";


        protected override string BuildFailedCondition => "errors";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }

}

