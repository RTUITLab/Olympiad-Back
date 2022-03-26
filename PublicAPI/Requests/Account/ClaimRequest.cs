using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PublicAPI.Requests.Account
{
    public class ClaimRequest
    {
        public const string TRIMMED_WORD = @"^\S(.*\S|)$";
        [Required]
        [MinLength(1)]
        [RegularExpression(TRIMMED_WORD)]
        public string Type { get; set; }
        [Required]
        [MinLength(1)]
        [RegularExpression(TRIMMED_WORD)]
        public string Value { get; set; }

        public static string PackClaimsToUrl(IEnumerable<ClaimRequest> claims)
        {
            if (claims?.Any() != true)
            {
                return null;
            }
            var stringBuilder = new StringBuilder();
            foreach (var claim in claims)
            {
                stringBuilder.Append(WebUtility.UrlEncode(claim.Type));
                stringBuilder.Append(':');
                stringBuilder.Append(WebUtility.UrlEncode(claim.Value));
                stringBuilder.Append(';');
            }
            return stringBuilder.ToString();
        }
        public static IEnumerable<ClaimRequest> ParseClaimsFromUrl(string fromUrlRow)
        {
            if (string.IsNullOrWhiteSpace(fromUrlRow))
            {
                return null;
            }
            return fromUrlRow.Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Split(":"))
                .Where(t => t.Length == 2)
                .Select(t => new ClaimRequest
                {
                    Type = WebUtility.UrlDecode(t[0]),
                    Value = WebUtility.UrlDecode(t[1]),
                });
        }
    }
}
