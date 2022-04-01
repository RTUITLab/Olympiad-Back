using Olympiad.Shared;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;

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
        public DateTimeOffset SendingTime { get; set; }
        public DateTimeOffset? StartCheckingTime { get; set; }
        public DateTimeOffset? CheckedTime { get; set; }
        public List<SolutionDocumentResponse> Documents { get; set; }
    }
}
