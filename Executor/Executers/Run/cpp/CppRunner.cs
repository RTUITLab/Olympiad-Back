using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Executor.Executers.Run;
using Models;

namespace Executor.Executers.Run.Cpp
{
    [Language("cpp")]
    class CppRunner : ProgramRunner
    {
        public CppRunner(Action<Guid, SolutionStatus> proccessSolution) : base(proccessSolution)
        {
        }
    }
}
