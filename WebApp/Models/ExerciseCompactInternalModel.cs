using Models.Exercises;
using Olympiad.Shared.Models;
using PublicAPI.Responses;
using PublicAPI.Responses.Challenges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class ExerciseCompactInternalModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public SolutionStatus Status { get; set; }
        public string ExerciseTask { get; set; }

        public ChallengeViewMode ChallengeViewMode { get; set; }
        public SolutionStatus? GetStatus()
        {
            if (ChallengeViewMode == ChallengeViewMode.Hidden)
            {
                return null;
            }
            return Status;
        }

        public HiddenSolutionStatus? GetHiddenStatus()
        {
            if (ChallengeViewMode == ChallengeViewMode.Open)
            {
                return null;
            }
            if (Enum.IsDefined(typeof(HiddenSolutionStatus), (int)Status))
            {
                return (HiddenSolutionStatus)(int)Status;
            }
            return HiddenSolutionStatus.Accepted;
        }
    }
}
