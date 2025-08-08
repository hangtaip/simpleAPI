namespace TransactionApi;

public interface IAppLogger
{
    void Debug(string message);
    void Error(string message, Exception? exception = null);
    void Info(string message);
    void Trace(string message);
    void Warning(string message, Exception? exception = null);
}

public class AppLogger : IAppLogger
{
    private readonly ILogger _logger;
    private readonly bool _loggingEnabled;

    public AppLogger(ILogger<AppLogger> logger, bool loggingEnabled = true)
    {
        _logger = logger;
        _loggingEnabled = loggingEnabled;
    }

    public void Debug(string message)
    {
        if (!_loggingEnabled) return;
        _logger.LogDebug(message);
    }
    public void Error(string message, Exception? exception = null)
    {
        if (!_loggingEnabled) return;
        _logger.LogError(exception, message);
    }
    public void Info(string message)
    {
        if (!_loggingEnabled) return;
        _logger.LogInformation(message);
    }
    public void Trace(string message)
    {
        if (!_loggingEnabled) return;
        _logger.LogTrace(message);
    }

    public void Warning(string message, Exception? exception = null)
    {
        if (!_loggingEnabled) return;
        _logger.LogWarning(exception, message);
    }

}
