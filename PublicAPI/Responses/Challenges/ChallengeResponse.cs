using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Challenges
{
    public class ChallengeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public TimeSpan? ToStart => StartTime == null ? null : StartTime - DateTimeOffset.UtcNow;
        public TimeSpan? ToEnd => EndTime == null ? null : EndTime - DateTimeOffset.UtcNow;
        public ChallengeAccessType ChallengeAccessType { get; set; }
    }
}
