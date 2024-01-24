using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Linq;

namespace Executor.Executers.Build;

class DockerBuildImageErrorProperty : BuildProperty
{
    public override bool IsCompilationFailed(IEnumerable<JSONMessage> logs) 
        => ContainsErrorOrErrorMessage(logs) || ContainsErrorWordInStream(logs);

    private static bool ContainsErrorOrErrorMessage(IEnumerable<JSONMessage> logs) 
        => logs.Any(l => !string.IsNullOrEmpty(l.ErrorMessage) || l.Error is not null);

    private static bool ContainsErrorWordInStream(IEnumerable<JSONMessage> logs) 
        => logs.Any(l => l.Stream?.Contains("\"error\":") == true) && logs.Any(l => l.Stream?.Contains("\"errorDetail\":") == true);
}
