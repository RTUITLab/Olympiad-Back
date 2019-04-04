using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Models.Exercises;

namespace Models.Solutions
{
    public class Solution
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public string Raw { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
        public SolutionStatus Status { get; set; }
        public DateTime SendingTime { get; set; }
        public DateTime? StartCheckingTime { get; set; }
        public DateTime? CheckedTime { get; set; }

        public List<SolutionCheck> SolutionChecks { get; set; }
    }
}
