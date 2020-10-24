using Models.Checking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Olympiad.Admin
{
    public static class Extensions
    {
        public static string GetStreamFromLog(this SolutionBuildLog buildLog)
        {
            return string.Join('\n', buildLog.Log
                .Split("\n")
                .Select(row => { try { return JsonDocument.Parse(row).RootElement; } catch { return default; } })
                .Where(e => e.ValueKind == JsonValueKind.Object)
                .Where(d => d.TryGetProperty("stream", out _))
                .Select(d => d.GetProperty("stream").GetString()));
        }

        private static readonly Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        public static string ConvertCyrillic(this string value)
        {
            return reg.Replace(value, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        }
    }
}
