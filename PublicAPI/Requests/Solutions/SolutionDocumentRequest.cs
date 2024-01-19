using ByteSizeLib;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace PublicAPI.Requests.Solutions
{
    public class SolutionDocumentRequest
    {
        public string Name { get; set; }
        public ByteSize Size { get; set; }
        public string MimeType { get; set; }
        public Stream Content { get; set; }

    }
}
