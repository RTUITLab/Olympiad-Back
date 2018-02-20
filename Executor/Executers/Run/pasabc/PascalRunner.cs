using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Executor.Executers.Run.PascalABC
{
    [Language("pasabc")]
    class PascalRunner : ProgramRunner
    {
        public PascalRunner(Action proccessSolution) : base(proccessSolution)
        {
        }
    }
}
