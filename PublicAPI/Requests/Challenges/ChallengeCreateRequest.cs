using Olympiad.Shared.Models;
using System;

namespace PublicAPI.Requests.Challenges
{
    public class ChallengeCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public ChallengeAccessType ChallengeAccessType { get; set; }
    }
}
