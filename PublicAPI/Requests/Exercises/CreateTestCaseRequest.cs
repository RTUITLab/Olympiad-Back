using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Exercises
{
    public  class CreateTestCaseRequest
    {
        [Required]
        [MaxLength(ExerciseDataLimitations.MAX_IN_DATA_LENGTH)]
        public string In { get; set; }
        [Required]
        [MaxLength(ExerciseDataLimitations.MAX_OUT_DATA_LENGTH)]
        public string Out { get; set; }
    }
}
