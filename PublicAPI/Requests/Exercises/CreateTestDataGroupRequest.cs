using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Exercises
{
    public class CreateTestDataGroupRequest
    {
        [Required]
        [MaxLength(ExerciseDataLimitations.MAX_GROUP_TITLE_LENGTH)]
        public string Title {get; set;}
        [Range(0, ExerciseDataLimitations.MAX_GROUP_SCORE)]
        public int Score {get; set;}
        public bool IsPublic {get; set;}
        public List<CreateTestCaseRequest> Cases { get; set; }
    }
}
