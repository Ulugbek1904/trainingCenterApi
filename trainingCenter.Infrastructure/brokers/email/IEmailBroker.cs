using System.Threading.Tasks;

namespace trainingCenter.Infrastructure.brokers.email
{
    public interface IEmailBroker
    {
        ValueTask SendEmailAsync(string email, string subject, string body);
    }
}