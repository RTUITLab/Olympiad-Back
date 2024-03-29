﻿using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Responses.Solutions
{
    public class SolutionDocumentResponse
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public ByteSize Size { get; set; }
        [MaxLength(255)]
        public string MimeType { get; set; }
    }
}
