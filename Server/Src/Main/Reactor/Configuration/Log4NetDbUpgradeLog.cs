using DbUp.Engine.Output;
using log4net;

namespace Src.Main.Reactor.Configuration;

public class Log4NetDbUpgradeLog : IUpgradeLog
{
  private readonly ILog _log;

  public Log4NetDbUpgradeLog(ILog log)
  {
    _log = log;
  }

  public void LogTrace(string format, params object[] args)
  {
    if (_log.IsInfoEnabled)
    {
      _log.DebugFormat(format, args);
    }
  }

  public void LogDebug(string format, params object[] args)
  {
    if (_log.IsInfoEnabled)
    {
      _log.DebugFormat(format, args);
    }
  }

  public void LogInformation(string format, params object[] args)
  {
    if (_log.IsInfoEnabled)
    {
      _log.InfoFormat(format, args);
    }
  }

  public void LogWarning(string format, params object[] args)
  {
    if (_log.IsWarnEnabled)
    {
      _log.WarnFormat(format, args);
    }
  }

  public void LogError(string format, params object[] args)
  {
    if (_log.IsErrorEnabled)
    {
      _log.ErrorFormat(format, args);
    }
  }

  public void LogError(Exception ex, string format, params object[] args)
  {
    if (_log.IsInfoEnabled)
    {
      _log.ErrorFormat(format, args, ex);
    }
  }
}
