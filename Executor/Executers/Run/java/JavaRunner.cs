using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Models;

namespace Executor.Executers.Run.Java
{
    [Language("java")]
    class JavaRunner : ProgramRunner
    {
        public JavaRunner(Action<Guid, SolutionStatus> proccessSolution) : base(proccessSolution)
        {
        }
    }
}
