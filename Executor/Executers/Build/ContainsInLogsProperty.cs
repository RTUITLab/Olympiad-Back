using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace Executor.Executers.Build;

class ContainsInLogsProperty : DockerBuildImageErrorProperty
{
    public string BuildFailedCondition { get; set; }

    public override bool IsCompilationFailed(IEnumerable<JSONMessage> logs) =>
        base.IsCompilationFailed(logs) ||
        logs.Any(l => l.Stream?.Contains(BuildFailedCondition) == true);
}
