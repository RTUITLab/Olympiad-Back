using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Exercise
    {
        public string ExerciseName { get; set; }
        public Guid ExerciseID { get; set; }
        public string ExerciseTask { get; set; }
        public int Score { get; set; }
    }
}
