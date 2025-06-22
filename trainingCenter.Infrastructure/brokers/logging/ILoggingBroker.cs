namespace trainingCenter.Infrastructure.brokers.logging
{
    public interface ILoggingBroker
    {
        void LogInformation(string message);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message);
        void LogError(string message, Exception exception);
        void LogDebug(string message);
    }
}