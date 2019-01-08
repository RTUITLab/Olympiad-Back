using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class DebugEmailService : IEmailSender
    {
        private readonly ILogger<DebugEmailService> logger;

        public DebugEmailService(ILogger<DebugEmailService> logger)
        {
            this.logger = logger;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            logger.LogInformation($"sending email to {email}, subject: {subject}, message: {message}");
            return Task.CompletedTask;
        }

        public Task SendEmailConfirm(string email, string url)
        {
            logger.LogInformation($"sending confirmation email to {email}, url: {url}");
            return Task.CompletedTask;
        }
    }
}
