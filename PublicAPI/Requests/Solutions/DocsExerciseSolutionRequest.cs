using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Solutions
{
    public class DocsExerciseSolutionRequest
    {
        [Required]
        [MinLength(1)]
        public List<SolutionDocumentRequest> Files { get; set; }
    }
}
