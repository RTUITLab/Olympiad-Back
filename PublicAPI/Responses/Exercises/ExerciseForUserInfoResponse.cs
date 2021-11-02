using Olympiad.Shared.Models;
using System;

namespace PublicAPI.Responses.Exercises
{
    public class ExerciseForUserInfoResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public SolutionStatus? Status { get; set; }
        public HiddenSolutionStatus? HiddenStatus { get; set; }
    }
}
