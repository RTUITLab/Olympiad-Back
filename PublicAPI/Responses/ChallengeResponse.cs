using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class ChallengeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<ExerciseCompactResponse> Exercises { get; set; }
    }
}
