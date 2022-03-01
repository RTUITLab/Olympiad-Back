using PublicAPI.Responses.Challenges;
using System;
using System.Collections.Generic;
using PublicAPI.Responses.Solutions;

namespace PublicAPI.Responses.ExerciseTestData
{
    public class ExerciseInfo : ExerciseCompactResponse
    {
        public string ExerciseTask { get; set; }

        public Guid ChallengeId { get; set; }
        public ChallengeResponse Challenge { get; set; }
    }
}
