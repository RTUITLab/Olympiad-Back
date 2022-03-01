using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class AttachmentResponse
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public void Deconstruct(out string fileName, out string mimeType)
        {
            fileName = FileName;
            mimeType = MimeType;
        }
    }
}
