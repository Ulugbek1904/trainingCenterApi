using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using trainingCenter.Common.Exceptions;

namespace trainingCenter.Infrastructure.brokers.email
{
    public class EmailBroker : IEmailBroker
    {
        private readonly IConfiguration configuration;

        public EmailBroker(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async ValueTask SendEmailAsync(string email, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
                throw new ValidationException("Subject or body cannot be empty.");

            string fromEmail = configuration["Smtp:From"] ??
                throw new ConfigurationException("SMTP From email is not configured.");

            string senderName = configuration["Smtp:SenderName"] ?? "O'quv Markazi";

            string host = configuration["Smtp:Host"] ??
                throw new ConfigurationException("SMTP Host is not configured.");

            string password = configuration["Smtp:Password"] ??
                throw new ConfigurationException("SMTP Password is not configured.");

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var smtpClient = new SmtpClient
            {
                Host = host,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                throw new EmailException("Failed to send email.", ex);
            }
        }
    }
}