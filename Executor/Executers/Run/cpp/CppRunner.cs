using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Executor.Executers.Run;
using Models;

namespace Executor.Executers.Run.Cpp
{
    [Language("cpp")]
    class CppRunner : ProgramRunner
    {
        public CppRunner(Func<Guid, SolutionStatus, Task> processSolution) : base(processSolution)
        {
        }
    }
}
