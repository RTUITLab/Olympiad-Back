using System;
using System.Collections.Generic;
using System.Text;

namespace Executor.Models.Settings
{
    public class StartSettings
    {
        public string Address { get; set; }
        public string DockerEndPoint { get; set; }
        public PrivateRegistrySettings PrivateDockerRegistry { get; set; }
        public string VersionString { get; set; }
    }
}
