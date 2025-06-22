using Microsoft.Extensions.Logging;
using trainingCenter.Infrastructure.brokers.logging;

namespace TrainingCenterApi.Infrastructure.Brokers
{
    public class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger)
        {
            this.logger = logger;
        }

        public void LogDebug(string message) => logger.LogDebug(message);
        public void LogError(string message, Exception exception) => logger.LogError(exception, message);
        public void LogInformation(string message) => logger.LogInformation(message);
        public void LogInformation(string message, params object[] args) => logger.LogInformation(message, args);
        public void LogTrace(string message) => logger.LogTrace(message);
        public void LogWarning(string message) => logger.LogWarning(message);
    }
}