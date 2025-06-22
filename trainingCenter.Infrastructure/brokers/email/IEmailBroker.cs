namespace trainingCenter.Infrastructure.brokers.email
{
    public interface IEmailBroker
    {
        ValueTask SendPasswordResetEmailAsync(string email, string PasswordResetOtp);
    }
}