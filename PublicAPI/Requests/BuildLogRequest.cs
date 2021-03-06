using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Requests
{
    public class BuildLogRequest
    {
        public string RawBuildLog { get; set; }
        public string PrettyBuildLog { get; set; }
    }
}
