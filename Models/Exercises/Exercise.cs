using Models.Solutions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Exercises
{
    public class Exercise
    {
        public string ExerciseName { get; set; }
        public Guid ExerciseID { get; set; }
        public string ExerciseTask { get; set; }

        public Guid ChallengeId { get; set; }
        public Challenge Challenge { get; set; }

        public List<ExerciseDataGroup> ExerciseDataGroups { get; set; }
        public List<Solution> Solutions { get; set; }
    }
}
