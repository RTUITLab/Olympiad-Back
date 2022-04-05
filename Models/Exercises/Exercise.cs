using Models.Solutions;
using Olympiad.Shared;
using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Exercises
{
    public class Exercise
    {
        public Guid ExerciseID { get; set; }
        [Required]
        [MaxLength(ExerciseLimitations.MAX_EXERCISE_TITLE_LENGTH)]
        public string ExerciseName { get; set; }
        [Required]
        [MaxLength(ExerciseLimitations.MAX_EXERCISE_TASK_LENGTH)]
        public string ExerciseTask { get; set; }
        /// <summary>
        /// Description for system administration, now visible to users
        /// </summary>
        [MaxLength(ExerciseLimitations.MAX_EXERCISE_INTERNAL_DESCRIPTION_LENGTH)]
        public string InternalDescription { get; set; }
        [Required]
        public ExerciseType Type { get; set; }


        [Column(TypeName = "jsonb")]
        public ExerciseRestrictions Restrictions { get; set; }

        public Guid ChallengeId { get; set; }
        public Challenge Challenge { get; set; }

        public List<ExerciseDataGroup> ExerciseDataGroups { get; set; }
        public List<Solution> Solutions { get; set; }
    }
}
