using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Solutions
{
    public class SolutionAnalyticCompactResponse
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public DateTimeOffset SendingTime { get; set; }
        public SolutionStatus Status { get; set; }
        public int? Score { get; set; }
    }
}
