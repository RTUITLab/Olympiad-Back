using System;
using System.Collections.Generic;
using System.Text;
using Models.Solutions;

namespace Models
{
    public class SolutionCheck
    {
        public Guid Id { get; set; }
        public DateTime CheckedTime { get; set; }
        public string ExampleIn { get; set; }
        public string ExampleOut { get; set; }

        public string ProgramOut { get; set; }
        public string ProgramErr { get; set; }
        public TimeSpan Duration { get; set; }

        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; }
    }
}
