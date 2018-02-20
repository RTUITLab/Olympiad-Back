using System;
using System.Collections.Generic;
using System.Text;

namespace Executor.Executers.Run.python
{
    [Language("python")]
    class PythonRunner : ProgramRunner
    {
        public PythonRunner(Action proccessSolution) : base(proccessSolution)
        {
        }
    }
}
