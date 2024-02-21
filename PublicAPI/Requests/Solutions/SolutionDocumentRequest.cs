using ByteSizeLib;
using System;
using System.IO;

namespace PublicAPI.Requests.Solutions;

public class SolutionDocumentRequest
{
    public string Name { get; set; }
    public ByteSize Size { get; set; }
    public string MimeType { get; set; }
    public Func<Stream> ContentFunc { get; set; }

}
