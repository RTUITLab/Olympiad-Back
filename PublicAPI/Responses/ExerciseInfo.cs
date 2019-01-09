using PublicAPI.Responses.Challenges;
using System;
using System.Collections.Generic;

namespace PublicAPI.Responses
{
    public class ExerciseInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public string TaskText { get; set; }

        public Guid ChallengeId { get; set; }
        public ChallengeCompactResponse Challenge { get; set; }

        public IEnumerable<SolutionResponse> Solutions { get; set; }
    }
}
