using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using trainingCenter.Common.Exceptions;
using trainingCenter.Infrastructure.brokers.email;

public class EmailBroker : IEmailBroker
{
    private readonly IConfiguration configuration;

    public EmailBroker(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async ValueTask SendPasswordResetEmailAsync(string email, string passwordResetOtp)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email cannot be empty.");
        if (string.IsNullOrWhiteSpace(passwordResetOtp))
            throw new ValidationException("Password reset OTP cannot be empty.");

        string fromEmail = configuration["Smtp:From"] ?? 
            throw new ConfigurationException("SMTP From email is not configured.");

        string senderName = configuration["Smtp:SenderName"] ?? "Smart Manager";

        string host = configuration["Smtp:Host"] ??
            throw new ConfigurationException("SMTP Host is not configured.");

        string password = configuration["Smtp:Password"] ?? 
            throw new ConfigurationException("SMTP Password is not configured.");

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, senderName),
            Subject = "Password Reset Request",
            Body = $"<h2>Password Reset</h2><p>Your OTP: <strong>{passwordResetOtp}</strong></p>" +
                $"<p>This OTP is valid for 10 minutes.</p>",
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