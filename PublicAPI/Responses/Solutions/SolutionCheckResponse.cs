using System;

namespace PublicAPI.Responses.Solutions
{
    public class SolutionCheckResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset CheckedTime { get; set; }
        public string ExampleIn { get; set; }
        public string ExampleOut { get; set; }

        public string ProgramOut { get; set; }
        public string ProgramErr { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
