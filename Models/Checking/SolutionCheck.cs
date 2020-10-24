using System;
using System.Collections.Generic;
using System.Text;
using Models.Solutions;
using Olympiad.Shared.Models;

namespace Models.Checking
{
    public class SolutionCheck
    {
        public Guid Id { get; set; }
        public DateTimeOffset CheckedTime { get; set; }
        public string ExampleIn { get; set; }
        public string ExampleOut { get; set; }

        public string ProgramOut { get; set; }
        public string ProgramErr { get; set; }
        public TimeSpan Duration { get; set; }
        public SolutionStatus Status { get; set; }

        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; }
    }
}
