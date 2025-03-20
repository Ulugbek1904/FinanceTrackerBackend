namespace FinanceTracker.Infrastructure.Brokers.Logging
{
    public interface ILoggingBroker
    {
        void LogInformation(string message);
        void LogInformation(string message, Exception exception);
        void LogError(Exception exception);
    }
}
