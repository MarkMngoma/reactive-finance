using System;
using Server.Main.Reactor.Clients;

namespace Server.Main.Reactor.Configuration.Modules;

public static class HttpClientModule
{
  public static void Configure(IServiceCollection services)
  {
    services.AddSingleton<BasePayFastHttpClient, FxHttpClient>();
    services.AddSingleton<FxHttpClient>();
  }
}
