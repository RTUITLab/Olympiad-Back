using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Dump
{
    public class SolutionDumpView
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public string Raw { get; set; }
        public string UserId { get; set; }
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int ExerciseScore { get; set; }
        public SolutionStatus Status { get; set; }
        public DateTime SendingTime { get; set; }
        public DateTime? StartCheckingTime { get; set; }
        public DateTime? CheckedTime { get; set; }
    }
}
