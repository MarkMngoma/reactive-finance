using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DbUp;
using log4net;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;
using Server.Main.Reactor.Handlers.CrossCutting;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Server.Main.Reactor.Configuration.Modules;

public static class ServerModule
{
  public static void Configure(IServiceCollection services, IConfiguration configuration)
  {
    services.AddHttpClient();
    var connectionString = configuration.GetConnectionString("FinanceDatabase");
    ApplyDatabaseMigrations(connectionString);

    services.AddSingleton(e =>
    {
      var connection = new MySqlConnection(connectionString);
      Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
      var compiler = new MySqlCompiler();
      var queryFactory = new QueryFactory(connection, compiler)
      {
        Logger = compiled => LogManager.GetLogger(typeof(ServerModule)).Debug($"Executing SQL query ===> : {compiled}")
      };
      return queryFactory;
    });

    services.AddExceptionHandler<GlobalExceptionHandler>();
    services.AddProblemDetails();
    services.AddEndpointsApiExplorer();

    services.AddControllers().AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
      options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc("v1", new OpenApiInfo
      {
        Version = "v1",
        Title = "Subscription Reactor API V1",
        Description = "REST API for managing financial product subscriptions and transactions"
      });
      options.SwaggerDoc("v2", new OpenApiInfo
      {
        Version = "v2",
        Title = "Subscription Reactor API V2",
        Description = "REST API for managing financial product subscriptions and transactions"
      });
      options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    });
  }

  private static void ApplyDatabaseMigrations(string? connectionString)
  {
    DeployChanges.To.MySqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogTo(new Log4NetDbUpgradeLog(LogManager.GetLogger(typeof(ServerModule))))
        .Build()
        .PerformUpgrade();
  }
}
