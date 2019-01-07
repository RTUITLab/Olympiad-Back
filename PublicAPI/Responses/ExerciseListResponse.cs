using Shared.Models;
using System;

namespace PublicAPI.Responses
{
    public class ExerciseListResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public SolutionStatus Status { get; set; }
    }
}
