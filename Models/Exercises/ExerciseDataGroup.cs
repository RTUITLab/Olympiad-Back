using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Exercises
{
    public class ExerciseDataGroup
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
        public bool IsPublic { get; set; }

        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public List<ExerciseData> ExerciseDatas { get; set; }
    }
}
