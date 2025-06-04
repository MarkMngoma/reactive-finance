using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.ApplicationInsights.Extensibility;
using Server.Main.Reactor.Configuration.Providers;

namespace Server.Main.Reactor.Configuration.Extensions;

public static class Log4NetExtensions
{
  public static void AddLog4Net(this ILoggingBuilder loggingBuilder, WebApplicationBuilder builder)
  {
    var configPath = "Infrastructure/Configuration";
    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
    if (builder.Environment.IsProduction())
    {
      var configuration = TelemetryConfiguration.CreateDefault();
      configuration.ConnectionString = builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString");
      XmlConfigurator.Configure(logRepository, new FileInfo($"{configPath}/log4net.Production.config"));
    }
    else
    {
      XmlConfigurator.Configure(logRepository, new FileInfo($"{configPath}/log4net.config"));
    }

    loggingBuilder.AddProvider(new Log4NetLoggerProvider());
  }
}
