using System;
using Server.Main.Reactor.Configuration.Extensions;

namespace Server.Main.Reactor.Configuration.Modules;

public static class LoggingModule
{
  public static void ConfigureLogging(WebApplicationBuilder builder)
  {
    builder.Logging.ClearProviders();
    builder.Logging.AddLog4Net(builder);
  }
}

