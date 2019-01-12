using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class RecaptchaResult
    {
        public bool Success { get; set; }
        public string HostName { get; set; }
        [JsonProperty("challenge_ts")]
        public DateTime ChallengeLoadTs { get; set; }
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
