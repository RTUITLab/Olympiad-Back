using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Solutions
{
    public class Solution
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public string Raw { get; set; }
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public SolutionStatus Status { get; set; }
        public DateTime Time { get; set; }
    }
}
