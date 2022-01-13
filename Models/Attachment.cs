using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Attachment
    {
        public string Path { get; set; }
        public int Size { get; set; }
        public string FileName => System.IO.Path.GetFileName(Path);
        public string Link { get; set; }
    }
}
