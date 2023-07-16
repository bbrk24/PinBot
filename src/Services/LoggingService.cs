using Discord;

namespace PinBot.Services;

public interface ILoggingService
{
    Task LogAsync(LogMessage logMessage);
}

public class LoggingService : ILoggingService
{
    private readonly ILogger _logger;

    public LoggingService(
        ILogger<LoggingService> logger
    )
    {
        _logger = logger;
    }

    private LogLevel SeverityToLevel(LogSeverity severity) => (LogLevel)(5 - severity);

    public Task LogAsync(LogMessage logMessage)
    {
        return Task.Run(() =>
            _logger.Log(
                SeverityToLevel(logMessage.Severity),
                logMessage.Exception,
                "{0}: {1}",
                logMessage.Source,
                logMessage.Message
            )
        );
    }
}
