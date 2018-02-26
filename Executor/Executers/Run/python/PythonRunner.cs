using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Executor.Executers.Run.python
{
    [Language("python")]
    class PythonRunner : ProgramRunner
    {
        public PythonRunner(Action<Guid, SolutionStatus> proccessSolution) : base(proccessSolution)
        {
        }
    }
}
