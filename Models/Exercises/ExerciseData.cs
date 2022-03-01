using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Exercises
{
    public class ExerciseData
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(ExerciseDataLimitations.MAX_IN_DATA_LENGTH)]
        public string InData { get; set; }
        [Required]
        [MaxLength(ExerciseDataLimitations.MAX_OUT_DATA_LENGTH)]
        public string OutData { get; set; }
        
        public ExerciseDataGroup ExerciseDataGroup { get; set; }
        public Guid ExerciseDataGroupId { get; set; }
    }
}
