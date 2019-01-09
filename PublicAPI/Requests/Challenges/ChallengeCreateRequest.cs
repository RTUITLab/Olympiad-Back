using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Requests.Challenges
{
    public class ChallengeCreateRequest
    {
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ChallengeAccessType ChallengeAccessType { get; set; }
    }
}
