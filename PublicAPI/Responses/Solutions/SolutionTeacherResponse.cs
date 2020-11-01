using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Solutions
{
    public class SolutionTeacherResponse
    {
        public Guid SolutionId { get; set; }
        public string ExerciseName { get; set; }
        public int UserScore { get; set; }
        public int TotalScore { get; set; }
        public string Raw { get; set; }
        public List<SolutionResponse> Solutions { get; set; }
    }
}
