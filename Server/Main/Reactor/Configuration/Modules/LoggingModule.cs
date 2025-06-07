using log4net;
using Server.Main.Reactor.Configuration.Extensions;

namespace Server.Main.Reactor.Configuration.Modules;

public static class LoggingModule
{
  public static void Configure(WebApplicationBuilder builder)
  {
    var requestId = Guid.NewGuid().ToString();
    GlobalContext.Properties["NDC"] = requestId;
    builder.Logging.ClearProviders();
    builder.Logging.AddLog4Net(builder);
  }
}

