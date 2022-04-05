using PublicAPI.Responses.Challenges;
using System;
using System.Collections.Generic;
using PublicAPI.Responses.Solutions;

namespace PublicAPI.Responses.Exercises
{
    public class AdminExerciseInfo : ExerciseInfo
    {
        public string InternalDescription { get; set; }
    }
}
