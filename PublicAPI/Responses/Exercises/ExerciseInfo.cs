﻿using PublicAPI.Responses.Challenges;
using System;
using System.Collections.Generic;
using PublicAPI.Responses.Solutions;

namespace PublicAPI.Responses.Exercises
{
    public class ExerciseInfo : ExerciseCompactResponse
    {
        public string ExerciseTask { get; set; }
        public ExerciseRestrictionsResponse Restrictions { get; set; }

        public Guid ChallengeId { get; set; }
        public ChallengeResponse Challenge { get; set; }
    }
}
