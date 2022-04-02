using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Exercises
{
    public class UpdateCodeRestrictionsRequest
    {
        [Required]
        [MinLength(1)]
        public List<ProgramRuntime> AllowedRuntimes { get; set; }
    }
}
