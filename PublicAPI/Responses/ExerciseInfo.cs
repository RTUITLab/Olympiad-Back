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
        public string ExerciseTask { get; set; }

        public Guid ChallengeId { get; set; }
        public ChallengeResponse Challenge { get; set; }

        public IEnumerable<SolutionResponse> Solutions { get; set; }
    }
}
