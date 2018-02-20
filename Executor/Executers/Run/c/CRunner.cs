using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Executor.Executers.Run;
using Models;

namespace Executor.Executers.Run.C
{
    [Language("c")]
    class CRunner : ProgramRunner
    {
        public CRunner(Action proccessSolution) : base(proccessSolution)
        {
        }

        public override string DockerImageName => "runner:c";
    }
}
