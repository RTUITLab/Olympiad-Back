using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Challenges
{
    public class ChallengeCompactResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? EndTime { get; set; }
        public ChallengeAccessType ChallengeAccessType { get; set; }
    }
}
