using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using log4net;
using Microsoft.AspNetCore.HttpLogging;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using Src.Main.Reactor.Clients;
using Src.Main.Reactor.Configuration.Extensions;
using Src.Main.Reactor.Handlers.Business;
using Src.Main.Reactor.Handlers.CrossCutting;
using Src.Main.Reactor.Middleware;

namespace Src.Main.Reactor;

public class Program
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

  public static void Main(string[] args)
  {
    var requestId = Guid.NewGuid().ToString();
    GlobalContext.Properties["NDC"] = requestId;
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile($"Src/Main/Infrastructure/Configuration/application.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

    ConfigureLogging(builder);
    ConfigureServices(builder.Services, builder.Configuration);
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
    builder.Logging.AddLog4Net();
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

    services.AddTransient<QueryFactory>(e =>
    {
      var connectionString = configuration.GetConnectionString("FinanceDatabase");
      var connection = new MySqlConnection(connectionString);

      var compiler = new MySqlCompiler();
      var queryFactory = new QueryFactory(connection, compiler);
      queryFactory.Logger = compiled => {
        Logger.Debug($"Executing SQL query ===> : {compiled}");
      };
      return queryFactory;
    });

    services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      });

    services.AddEndpointsApiExplorer();
  }

  private static void ConfigureHandlerModules(IServiceCollection services)
  {
    services.AddSingleton<ContentResultHandler>();
    services.AddSingleton<ThrowableHandler>();
    services.AddSingleton<WriteCurrenciesHandler>();
    services.AddSingleton<QueryCurrenciesHandler>();
  }

  private static void ConfigureClientModules(IServiceCollection services)
  {
    services.AddSingleton<FxHttpClient>();
  }

  private static void ConfigureMiddleware(WebApplication app)
  {
    app.UseMiddleware<LoggingMiddleware>();

    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Home/Error");
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
  }
}
