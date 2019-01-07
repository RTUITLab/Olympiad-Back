using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class ChallengeCompactResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
