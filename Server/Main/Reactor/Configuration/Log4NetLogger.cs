using log4net;

namespace Server.Main.Reactor.Configuration;

public class Log4NetLogger : ILogger
{
  private readonly ILog _logger;

  public Log4NetLogger(string categoryName)
  {
    _logger = LogManager.GetLogger(categoryName);
  }

  IDisposable ILogger.BeginScope<TState>(TState state)
  {
    return NullScope.Instance;
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return logLevel switch
    {
      LogLevel.Trace or LogLevel.Debug => _logger.IsDebugEnabled,
      LogLevel.Information => _logger.IsInfoEnabled,
      LogLevel.Warning => _logger.IsWarnEnabled,
      LogLevel.Error => _logger.IsErrorEnabled,
      LogLevel.Critical => _logger.IsFatalEnabled,
      _ => false,
    };
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
      Func<TState, Exception?, string> formatter)
  {
    if (formatter == null) throw new ArgumentNullException(nameof(formatter));

    var logMessage = formatter(state, exception);

    switch (logLevel)
    {
      case LogLevel.Trace:
      case LogLevel.Debug:
        _logger.Debug(logMessage, exception);
        break;
      case LogLevel.Information:
        _logger.Info(logMessage, exception);
        break;
      case LogLevel.Warning:
        _logger.Warn(logMessage, exception);
        break;
      case LogLevel.Error:
        _logger.Error(logMessage, exception);
        break;
      case LogLevel.Critical:
        _logger.Fatal(logMessage, exception);
        break;
    }
  }

  private class NullScope : IDisposable
  {
    public static NullScope Instance { get; } = new NullScope();
    private NullScope() { }
    public void Dispose() { }
  }
}
