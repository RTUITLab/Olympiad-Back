using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Exercises
{
    public class AdminExerciseCompactResponse : ExerciseCompactResponse
    {
        public string InternalDescription { get; set; }
    }
}
