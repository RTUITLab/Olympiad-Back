using Ardalis.SmartEnum.SystemTextJson;
using Olympiad.Shared;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PublicAPI.Responses.Solutions.Analytics
{
    public class SolutionAnalyticCompactResponse
    {
        public Guid Id { get; set; }
        public ProgramRuntime Language { get; set; }
        public DateTimeOffset SendingTime { get; set; }
        public SolutionStatus Status { get; set; }
        public int? Score { get; set; }
        public ExerciseType ExerciseType { get; set; }
    }
}
