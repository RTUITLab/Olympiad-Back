using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Exercises
{
    public class UpdateExerciseRequest
    {
        [Required]
        [MaxLength(ExerciseLimitations.MAX_EXERCISE_TITLE_LENGTH)]
        public string Title { get; set; }
        [Required]
        [MaxLength(ExerciseLimitations.MAX_EXERCISE_TASK_LENGTH)]
        public string Task { get; set; }
    }
}
