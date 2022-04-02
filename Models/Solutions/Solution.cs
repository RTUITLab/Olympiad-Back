using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Models.Checking;
using Models.Exercises;
using Olympiad.Shared;
using Olympiad.Shared.Models;

namespace Models.Solutions
{
    public class Solution
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public ProgramRuntime Language { get; set; }

        public string Raw { get; set; }
        [Column(TypeName = "jsonb")]
        public SolutionDocuments DocumentsResult {get; set;}
        
        public SolutionStatus Status { get; set; }
        public DateTimeOffset SendingTime { get; set; }
        public DateTimeOffset? StartCheckingTime { get; set; }
        public DateTimeOffset? CheckedTime { get; set; }
        
        public int? TotalScore { get; set; }

        public List<SolutionCheck> SolutionChecks { get; set; }
        public List<SolutionBuildLog> SolutionBuildLogs { get; set; }
    }
}
