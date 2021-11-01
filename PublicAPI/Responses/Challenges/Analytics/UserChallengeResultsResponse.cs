using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Challenges.Analytics
{
    public class UserChallengeResultsResponse
    {
        public UserView User { get; set; }
        public Dictionary<string, int?> Scores { get; set; }
        public int? TotalScore { get; set; }
        public class UserView
        {
            public Guid Id { get; set; }
            public string StudentId { get; set; }
            public string FirstName { get; set; }
        }
    }
    
}
