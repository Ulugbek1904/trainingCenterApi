using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using trainingCenter.Infrastructure.brokers.email;

public class EmailBroker : IEmailBroker
{
    private readonly IConfiguration configuration;

    public EmailBroker(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async ValueTask SendPasswordResetEmailAsync(string email, string passwordResetOtp)
    {
        string fromEmail = configuration["Smtp:From"];
        string senderName = configuration["Smtp:SenderName"];
        string host = configuration["Smtp:Host"];
        string password = configuration["Smtp:Password"];

        var message = new MailMessage
        {
            From = new MailAddress(fromEmail, senderName),
            Subject = "Password Reset Request",
            Body = $"<strong>Here is your password reset OTP: {passwordResetOtp}</strong>",
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

        await smtpClient.SendMailAsync(message);
    }
}
