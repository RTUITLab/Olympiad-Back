using System;
using System.Collections.Generic;
using System.Text;

namespace Executor.Executers
{
    interface IWithDockerImage
    {
        string DockerImageName { get; }
    }
}
