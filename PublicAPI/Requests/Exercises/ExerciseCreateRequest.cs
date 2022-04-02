using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Exercises
{
    public class ExerciseCreateRequest
    {
        public Guid ChallengeId { get; set; }
        [Required]
        public ExerciseType Type { get; set; }
    }
}
