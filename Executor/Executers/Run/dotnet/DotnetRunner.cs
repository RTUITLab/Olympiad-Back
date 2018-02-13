using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Models;

namespace Executor.Executers.Run.dotnet
{
    [Language("csharp")]
    class DotnetRunner : ProgramRunner
    {
        public DotnetRunner(Action proccessSolution) : base(proccessSolution)
        {
        }

        protected override string DockerImageName => "runner:dotnet";
    }
}
