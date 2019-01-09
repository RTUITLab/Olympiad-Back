using Docker.DotNet;
using Models;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Executor.Executers.Run.python
{
    [Language("python")]
    class PythonRunner : ProgramRunner
    {
        public PythonRunner(Func<Guid, SolutionStatus, Task> processSolution, IDockerClient dockerClient) : base(processSolution, dockerClient)
        {
        }
    }
}
