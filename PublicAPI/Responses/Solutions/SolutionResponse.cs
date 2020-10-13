using Olympiad.Shared.Models;
using System;

namespace PublicAPI.Responses.Solutions
{
    public class SolutionResponse
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public SolutionStatus? Status { get; set; }
        public HiddenSolutionStatus? HiddenStatus { get; set; }
        public DateTime SendingTime { get; set; }
        public DateTime? StartCheckingTime { get; set; }
        public DateTime? CheckedTime { get; set; }
    }
}
