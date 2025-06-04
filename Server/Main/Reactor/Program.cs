using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DbUp;
using log4net;
using Microsoft.AspNetCore.HttpLogging;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using Server.Main.Reactor.Clients;
using Server.Main.Reactor.Configuration;
using Server.Main.Reactor.Configuration.Extensions;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.Business.Finance;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Middleware;

namespace Server.Main.Reactor;

public class Program
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

  public static void Main(string[] args)
  {
    var requestId = Guid.NewGuid().ToString();
    GlobalContext.Properties["NDC"] = requestId;
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile($"Infrastructure/Configuration/application.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

    ConfigureLogging(builder);
    ConfigureServices(builder.Services, builder.Configuration);
    ConfigureDomainModules(builder.Services);
    ConfigureHandlerModules(builder.Services);
    ConfigureClientModules(builder.Services);

    var app = builder.Build();
    ConfigureMiddleware(app);
    app.UseHttpLogging();
    app.Run();
  }

  private static void ConfigureLogging(WebApplicationBuilder builder)
  {
    builder.Logging.ClearProviders();
    builder.Logging.AddLog4Net(builder);
  }

  private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
  {
    services.AddHttpClient();
    services.AddHttpLogging(logging =>
    {
      logging.LoggingFields = HttpLoggingFields.All;
      logging.RequestHeaders.Add("sec-ch-ua");
      logging.ResponseHeaders.Add("X-App-Token");
      logging.MediaTypeOptions.AddText(MediaTypeNames.Application.Json);
      logging.RequestBodyLogLimit = 4096;
      logging.ResponseBodyLogLimit = 4096;
      logging.CombineLogs = true;
    });

    var connectionString = configuration.GetConnectionString("FinanceDatabase");
    ApplyDatabaseMigrations(connectionString);
    services.AddSingleton<QueryFactory>(e =>
    {
      var connection = new MySqlConnection(connectionString);
      Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
      var compiler = new MySqlCompiler();
      var queryFactory = new QueryFactory(connection, compiler)
      {
        Logger = compiled =>
        {
          Logger.Debug($"Executing SQL query ===> : {compiled}");
        }
      };
      return queryFactory;
    });

    services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      });

    services.AddEndpointsApiExplorer();
  }

  private static void ConfigureHandlerModules(IServiceCollection services)
  {
    services.AddSingleton<ContentResultUtil>();
    services.AddSingleton<WriteCurrenciesHandler>();
    services.AddSingleton<WriteBatchCurrenciesHandler>();
    services.AddSingleton<QueryCurrenciesHandler>();
  }

  private static void ConfigureDomainModules(IServiceCollection services)
  {
    services.AddSingleton<CurrencyDomainHandler>();
  }

  private static void ConfigureClientModules(IServiceCollection services)
  {
    services.AddSingleton<BasePayFastHttpClient, FxHttpClient>();
    services.AddSingleton<FxHttpClient>();
  }

  private static void ConfigureMiddleware(WebApplication app)
  {
    app.UseMiddleware<LoggingMiddleware>();

    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/ErrorResource/Error");
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseWebSockets();
    app.UseRouting();
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
  }

  private static void ApplyDatabaseMigrations(string? connectionString)
  {
    DeployChanges.To.MySqlDatabase(connectionString)
      .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
      .LogTo(new Log4NetDbUpgradeLog(Logger))
      .LogScriptOutput()
      .Build()
      .PerformUpgrade();
  }
}
