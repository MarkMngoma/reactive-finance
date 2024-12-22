namespace Src.Main.Reactor.Configuration.Providers;

public class Log4NetLoggerProvider : ILoggerProvider
  {
    public ILogger CreateLogger(string categoryName)
    {
      return new Log4NetLogger(categoryName);
    }

    public void Dispose()
    {
    }
}
