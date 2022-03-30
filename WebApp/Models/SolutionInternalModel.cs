using Olympiad.Shared;
using Olympiad.Shared.Models;
using PublicAPI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class SolutionInternalModel
    {
        public Guid Id { get; set; }
        public ProgramRuntime Language { get; set; }
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public SolutionStatus Status { get; set; }
        public DateTimeOffset SendingTime { get; set; }
        public DateTimeOffset? StartCheckingTime { get; set; }
        public DateTimeOffset? CheckedTime { get; set; }

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
