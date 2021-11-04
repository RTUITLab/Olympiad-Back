using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Solutions.Analytics
{
    public class BuildLogAnalyticsResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset BuildedTime { get; set; }
        public string Log { get; set; }
        public string PrettyLog { get; set; }
    }
}
