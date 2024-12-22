using log4net;

namespace Src.Main.Reactor.Configuration;

public class Log4NetLogger : ILogger
{
  private readonly ILog _logger;

  public Log4NetLogger(string categoryName)
  {
    _logger = LogManager.GetLogger(categoryName);
  }

  public IDisposable BeginScope<TState>(TState state)
  {
    return null;
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return true;
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
    Func<TState, Exception, string> formatter)
  {
    var logMessage = formatter(state, exception);
    switch (logLevel)
    {
      case LogLevel.Trace:
      case LogLevel.Debug:
        _logger.Debug(logMessage);
        break;
      case LogLevel.Information:
        _logger.Info(logMessage);
        break;
      case LogLevel.Warning:
        _logger.Warn(logMessage);
        break;
      case LogLevel.Error:
        _logger.Error(logMessage);
        break;
      case LogLevel.Critical:
        _logger.Fatal(logMessage);
        break;
      case LogLevel.None:
        break;
    }
  }
}

