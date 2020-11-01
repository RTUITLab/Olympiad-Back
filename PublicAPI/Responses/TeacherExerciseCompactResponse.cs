using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;

namespace PublicAPI.Responses
{
    public class TeacherExerciseCompactResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public SolutionStatus? Status { get; set; }
        public HiddenSolutionStatus? HiddenStatus { get; set; }
        public List<string> UserNames { get; set; }
    }
}
