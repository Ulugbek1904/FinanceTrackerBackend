using Serilog;

namespace FinanceTracker.Infrastructure.Brokers.Logging
{
    public class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger _logger;
        public LoggingBroker()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information()
                .CreateLogger();
        }

        public void LogError(Exception exception)
        {
            _logger.Error(exception, exception.Message);
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogInformation(string message, Exception exception)
        {
            _logger.Information(exception, message);
        }
    }
}
