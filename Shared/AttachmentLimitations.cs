using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Shared
{
    public static class AttachmentLimitations
    {
        public static ByteSize MaxAttachmentSize { get; } = ByteSize.FromMegaBytes(5);
    }
}
