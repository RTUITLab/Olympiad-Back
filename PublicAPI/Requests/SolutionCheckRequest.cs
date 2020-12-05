using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests
{
    public class SolutionCheckRequest
    {
        public string ExampleIn { get; set; }
        public string ExampleOut { get; set; }
        public string ProgramOut { get; set; }
        public string ProgramErr { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        public SolutionStatus Status { get; set; }
    }
}
