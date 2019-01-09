using Models;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Executor.Executers.Run.PascalABC
{
    [Language("pasabc")]
    class PascalRunner : ProgramRunner
    {
        public PascalRunner(Func<Guid, SolutionStatus, Task> processSolution) : base(processSolution)
        {
        }
    }
}
