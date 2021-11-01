using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Challenges
{
    public class ChallengeResponseWithAnalytics : ChallengeResponse
    {
        public int StartedExecutionCount { get; set; }
        public int InvitedCount { get; set; }
    }
}
