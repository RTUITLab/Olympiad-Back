using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Extensions;
using WebApp.Models;

namespace WebApp.Services.ReCaptcha
{
    public class DebugRecaptchaVerifier : IRecaptchaVerifier
    {
        private readonly ILogger<DebugRecaptchaVerifier> logger;

        public DebugRecaptchaVerifier(ILogger<DebugRecaptchaVerifier> logger)
        {
            this.logger = logger;
        }


        public Task<RecaptchaResult> Check(string token, string ip)
        {
            logger.LogWarning("Using Debug verifier for reCAPTCHA");
            return Task.FromResult(new RecaptchaResult
            {
                Success = true,
                ChallengeLoadTs = DateTime.UtcNow,
                ErrorCodes = new List<string>(),
                HostName = ""
            });
        }
    }
}
