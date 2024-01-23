using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace Executor.Executers.Build;

class DockerBuildImageErrorProperty : BuildProperty
{
    public override bool IsCompilationFailed(IEnumerable<JSONMessage> logs)
    {
        return logs.Any(l => l.Stream?.Contains("\"error\":") == true) && (logs.Any(l => l.Stream?.Contains("\"errorDetail\":") == true));
    }
}
