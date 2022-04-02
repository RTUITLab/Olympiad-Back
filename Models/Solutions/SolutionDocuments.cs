using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Solutions
{
    public class SolutionDocuments
    {
        public List<SolutionFile> Files { get; set; }
    }
    public class SolutionFile
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        /// <summary>
        /// File size in bytes
        /// </summary>
        public double Size { get; set; }
    }
}
