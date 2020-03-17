using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Services.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using WebApp.Models.Settings;

namespace WebApp.Services
{
    internal class EmailService : IEmailSender
    {
        private readonly EmailSettings options;

        public EmailService(IOptions<EmailSettings> options)
        {
            this.options = options.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", options.Email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var admin = new SmtpClient())
            {
                await admin.ConnectAsync(options.SmtpHost, options.SmtpPort, options.SmtpUseSsl);
                await admin.AuthenticateAsync(options.Email, options.Password);
                await admin.SendAsync(emailMessage);
                await admin.DisconnectAsync(true);
            }
        }

        public async Task SendEmailConfirm(string email, string url)
        {
            await SendEmailAsync(email, "Подтверждение регистрации", $"<a href=\"{url}\">Подтверждение почты</a>");
        }
    }
}

