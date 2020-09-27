using Olympiad.Shared.Models;
using System;

namespace PublicAPI.Requests.Challenges
{
    public class ChallengeCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ChallengeAccessType ChallengeAccessType { get; set; }
    }
}
