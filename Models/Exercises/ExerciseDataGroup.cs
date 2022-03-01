using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Exercises
{
    public class ExerciseDataGroup
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(ExerciseDataLimitations.MAX_GROUP_TITLE_LENGTH)]
        public string Title { get; set; }
        [Range(0, ExerciseDataLimitations.MAX_GROUP_SCORE)]
        public int Score { get; set; }
        public bool IsPublic { get; set; }

        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public List<ExerciseData> ExerciseDatas { get; set; }
    }
}
