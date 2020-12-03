using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Executor.Executers.Build
{
    class DockerBuildImageErrorProperty : BuildProperty
    {
        public override bool IsCompilationFailed(string logs)
        {
            return (logs?.Contains("\"error\":") == true) && (logs?.Contains("\"errorDetail\":") == true);
        }
    }
}
