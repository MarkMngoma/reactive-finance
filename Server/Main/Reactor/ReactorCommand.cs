using log4net;
using Server.Main.Reactor.Configuration.Extensions;
using Server.Main.Reactor.Configuration.Modules;
using Server.Main.Reactor.Configuration.Objects;

namespace Server.Main.Reactor;

public class ReactorCommand
{
  public void Run(string[] args)
  {
    var requestId = Guid.NewGuid().ToString();
    GlobalContext.Properties["NDC"] = requestId;

    var builder = WebApplication.CreateBuilder(args);
    ConfigResolver(builder);
    LoggingModule.ConfigureLogging(builder);
    ServerModule.ConfigureServices(builder.Services, builder.Configuration);
    BusinessModule.Configure(builder.Services);
    DomainModule.Configure(builder.Services);
    HttpClientModule.Configure(builder.Services);
    HazelcastModule.Configure(builder);

    var app = builder.Build();
    MiddlewareModule.Configure(app);

    app.Run();
  }

  private static void ConfigResolver(WebApplicationBuilder builder)
  {
    var configFile = $"Infrastructure/Configuration/config.{builder.Environment.EnvironmentName.ToLower()}.yaml";
    builder.Configuration.AddYamlFile(configFile, optional: false, reloadOnChange: true);
    builder.Services.AddConfig<IntegrationsConfig>(builder.Configuration, "IntegrationsConfig");
    builder.Services.AddConfig<TransactionConfig>(builder.Configuration, "TransactionConfig");
    builder.Services.AddConfig<ForexConfig>(builder.Configuration, "ForexConfig");
  }
}
