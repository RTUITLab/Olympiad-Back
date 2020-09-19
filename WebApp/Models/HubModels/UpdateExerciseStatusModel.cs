using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.HubModels
{
    public class UpdateExerciseStatusModel
    {
        public Guid ExerciseId { get; set; }
        public SolutionStatus ExerciseStatus { get; set; }
    }
}
