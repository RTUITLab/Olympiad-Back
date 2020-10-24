using Models.Solutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Checking
{
    public class SolutionBuildLog
    {
        public Guid Id { get; set; }
        public DateTime BuildedTime { get; set; }
        public string Log { get; set; }
        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; }
    }
}
