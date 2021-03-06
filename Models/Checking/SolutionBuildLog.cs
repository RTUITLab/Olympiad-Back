using Models.Solutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Checking
{
    public class SolutionBuildLog
    {
        public Guid Id { get; set; }
        public DateTimeOffset BuildedTime { get; set; }
        public string Log { get; set; }
        public string PrettyLog { get; set; }
        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; }
    }
}
