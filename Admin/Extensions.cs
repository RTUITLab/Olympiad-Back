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
        private static readonly Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        public static string ConvertCyrillic(this string value)
        {
            return reg.Replace(value, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        }
    }
}
