using Olympiad.Shared.Models;
using System;

namespace PublicAPI.Responses
{
    public class ExerciseCompactResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public int Status { get; set; }
        public string ExerciseTask { get; set; }
    }
}
