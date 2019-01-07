using System;
using System.Collections.Generic;

namespace PublicAPI.Responses
{
    public class ExerciseInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public string TaskText { get; set; }
        public IEnumerable<SolutionInfo> Solutions { get; set; }
    }
}
