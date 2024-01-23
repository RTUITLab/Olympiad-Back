using Docker.DotNet.Models;
using System.Collections.Generic;

namespace Executor.Executers.Build;

abstract class BuildProperty
{
    public abstract bool IsCompilationFailed(IEnumerable<JSONMessage> logs);
}
