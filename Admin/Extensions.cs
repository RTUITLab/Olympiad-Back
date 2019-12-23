using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    }
}
