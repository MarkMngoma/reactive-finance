using Hazelcast;
using Hazelcast.Networking;
using Server.Main.Reactor.Configuration.Extensions;
using Server.Main.Reactor.Handlers.CrossCutting;

namespace Server.Main.Reactor.Configuration.Modules;

public static class HazelcastModule
{
  public static void Configure(WebApplicationBuilder builder)
  {
    var loggerFactory = LoggerFactory.Create(logging => logging.AddLog4Net(builder));

    var address = builder.Configuration.GetValue<string>("Hazelcast:ServerAddress");
    var hazelcastOptions = new HazelcastOptionsBuilder()
        .WithLoggerFactory(_ => loggerFactory)
        .WithEnvironment(builder.Environment.EnvironmentName)
        .With(config =>
        {
          config.Networking.ReconnectMode = ReconnectMode.ReconnectAsync;
          config.Metrics.Enabled = true;

          if (builder.Environment.IsProduction())
          {
            config.Networking.Addresses.Add(address);
          }
        })
        .Build();

    builder.Services.AddSingleton(_ =>
        HazelcastClientFactory.StartNewClientAsync(hazelcastOptions)
        .GetAwaiter()
        .GetResult()
    );
    builder.Services.AddSingleton<ICachingHandler, CachingHandler>();
  }
}

