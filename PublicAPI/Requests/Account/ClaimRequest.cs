using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [RegularExpression(TRIMMED_WORD)]
        public string Value { get; set; }
    }
}
