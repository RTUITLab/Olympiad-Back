using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Executor.Executers.Build.Java
{
    [Language("java")]
    class JavaBuilder : ProgramBuilder
    {
        public JavaBuilder(Action proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution)
            : base(proccessSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Main.java";

        protected override string DockerImageName => "builder:java";

        protected override string BuildFailedCondition => "errors";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => startDir.FullName;
    }

}

