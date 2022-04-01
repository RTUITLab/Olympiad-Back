using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Solutions
{
    public class SolutionDocumentRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public ByteSize Size { get; set; }
        [Required]
        [MaxLength(255)]
        public string MimeType { get; set; }
    }
}
