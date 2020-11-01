using System;
using System.Collections.Generic;
using System.Text;
using Models.Checking;
using Models.Exercises;
using Olympiad.Shared.Models;

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
        public DateTimeOffset SendingTime { get; set; }
        public DateTimeOffset? StartCheckingTime { get; set; }
        public DateTimeOffset? CheckedTime { get; set; }

        public List<SolutionCheck> SolutionChecks { get; set; }
        public List<SolutionBuildLog> SolutionBuildLogs { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
