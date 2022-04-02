using ByteSizeLib;
using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Exercises
{
    public class UpdateDocsRestrictionsRequest
    {
        [Required]
        [MinLength(1)]
        public List<DocumentRestrictionRequest> Documents { get; set; }
    }
    public class DocumentRestrictionRequest
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
