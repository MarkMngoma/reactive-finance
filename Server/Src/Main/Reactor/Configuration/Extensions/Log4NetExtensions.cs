using System.Reflection;
using log4net;
using log4net.Config;
using Src.Main.Reactor.Configuration.Providers;

namespace Src.Main.Reactor.Configuration.Extensions;

public static class Log4NetExtensions
{
  public static void AddLog4Net(this ILoggingBuilder loggingBuilder)
  {
    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
    XmlConfigurator.Configure(logRepository, new FileInfo("Src/Main/Infrastructure/Configuration/log4net.config"));

    loggingBuilder.AddProvider(new Log4NetLoggerProvider());
  }
}
