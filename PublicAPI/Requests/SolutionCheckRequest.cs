using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests
{
    public class SolutionCheckRequest
    {
        [Required]
        public string ExampleIn { get; set; }
        [Required]
        public string ExampleOut { get; set; }
        [Required]
        public string ProgramOut { get; set; }
        [Required]
        public string ProgramErr { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
    }
}
