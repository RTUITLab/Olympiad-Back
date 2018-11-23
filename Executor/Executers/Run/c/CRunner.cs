using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Executor.Executers.Run;
using Models;

namespace Executor.Executers.Run.C
{
    [Language("c")]
    class CRunner : ProgramRunner
    {
        public CRunner(Func<Guid, SolutionStatus, Task> processSolution) : base(processSolution)
        {
        }
    }
}
