using PublicAPI.Responses.Exercises;
using PublicAPI.Responses.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Solutions.Analytics
{
    public class SolutionAnalyticsResponse : SolutionAnalyticCompactResponse
    {
        public ExerciseInfo Exercise { get; set; }
        public UserInfoResponse User { get; set; }
    }
}
