using Server.Main.Reactor.Configuration.Modules;

namespace Server.Main.Reactor;

public class ReactorCommand
{
  public void Run(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    LoggingModule.Configure(builder);
    ConfigModule.Configure(builder);
    ServerModule.Configure(builder.Services, builder.Configuration);
    BusinessModule.Configure(builder.Services);
    DomainModule.Configure(builder.Services);
    HttpClientModule.Configure(builder.Services);
    HazelcastModule.Configure(builder);

    var app = builder.Build();
    MiddlewareModule.Configure(app);
    app.Run();
  }
}
