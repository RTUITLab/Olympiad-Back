using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.Admin
{
    public static class RepairLogic
    {
        public static string UpdateSource(string source, string newSource)
        {
            var dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz");

            var header = $"// --- EDITED FOR CHECKING {dateStr} ---";
            var commented = string.Join('\n', source.Split("\n").Select(l => $"//{l}"));
            var codeSeparator = "// --- OLD SOURCE ---";

            return $"{header}\n{newSource}\n\n{codeSeparator}\n\n{commented}";
        }
    }
}
