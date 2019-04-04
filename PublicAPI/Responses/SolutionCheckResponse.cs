using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class SolutionCheckResponse
    {
        public Guid Id { get; set; }
        public DateTime CheckedTime { get; set; }
        public string ExampleIn { get; set; }
        public string ExampleOut { get; set; }

        public string ProgramOut { get; set; }
        public string ProgramErr { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
