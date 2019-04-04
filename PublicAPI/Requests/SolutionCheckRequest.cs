using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Requests
{
    public class SolutionCheckRequest
    {
        public string ExampleIn { get; set; }
        public string ExampleOut { get; set; }

        public string ProgramOut { get; set; }
        public string ProgramErr { get; set; }
    }
}
