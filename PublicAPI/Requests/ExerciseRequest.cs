using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAPI.Requests
{
    public class ExerciseRequest
    {
        public string ExerciseName { get; set; }
        public string ExerciseTask { get; set; }
        public int Score { get; set; } = -1;
    }
}
