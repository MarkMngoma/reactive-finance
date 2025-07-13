using Server.Main.Reactor.Configuration.Extensions;
using Server.Main.Reactor.Configuration.Objects;

namespace Server.Main.Reactor.Configuration.Modules;

public class ConfigModule
{
  public static void Configure(WebApplicationBuilder builder)
  {
    var configFile = $"Infrastructure/Configuration/config.{builder.Environment.EnvironmentName.ToLower()}.yaml";
    builder.Configuration
      .SetBasePath(AppContext.BaseDirectory)
      .AddYamlFile(configFile, optional: false, reloadOnChange: true);
    builder.Services.AddConfig<IntegrationsConfig>(builder.Configuration, "IntegrationsConfig");
    builder.Services.AddConfig<TransactionConfig>(builder.Configuration, "TransactionConfig");
    builder.Services.AddConfig<ForexConfig>(builder.Configuration, "ForexConfig");
  }
}
