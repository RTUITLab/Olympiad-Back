using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Solutions.Analytics
{
    public class SolutionTestGroupResulResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int GroupScore { get; set; }
        public SolutionStatus BestStatus { get; set; }
        public bool IsPublic { get; set; }
        public int? ResultScoreScore => BestStatus == SolutionStatus.Successful ? GroupScore : (int?)null;
    }
}
