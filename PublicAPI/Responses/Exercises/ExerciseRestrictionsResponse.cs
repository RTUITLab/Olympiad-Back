using ByteSizeLib;
using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Responses.Exercises
{
    public class ExerciseRestrictionsResponse
    {
        public CodeRestrictionsResponse Code { get; set; }
        public DocsRestrictionsResponse Docs { get; set; }
    }

    public class CodeRestrictionsResponse
    {
        public List<ProgramRuntime> AllowedRuntimes { get; set; }
    }
    public class DocsRestrictionsResponse
    {
        public List<DocumentRestrictionResponse> Documents { get; set; }
    }
    public class DocumentRestrictionResponse
    {
        [Required]
        [MinLength(1)]
        public List<string> AllowedExtensions { get; set; }
        public ByteSize MaxSize { get; set; }
        /// <summary>
        /// Document title for user
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        /// <summary>
        /// Document description for user
        /// </summary>
        [MaxLength(100)] 
        public string Description { get; set; }
    }
}
