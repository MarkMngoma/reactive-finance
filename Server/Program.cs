using System.Reflection;
using System.Text.Json;
using log4net;
using log4net.Config;
using MySql.Data.MySqlClient;
using server.za.co.bitbridge.clients;
using server.za.co.finance.handlers;
using server.za.co.finance.handlers.crosscutting;
using server.za.co.finance.middleware;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Server
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      ConfigureLogging(builder);
      ConfigureServices(builder.Services);
      ConfigureHandlerModules(builder.Services);
      ConfigureClientModules(builder.Services);

      var app = builder.Build();
      ConfigureMiddleware(app);
      app.Run();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
      builder.Logging.ClearProviders();
      builder.Logging.AddLog4Net();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
      services.AddHttpClient();
      services.AddHttpLogging(options => { });

      services.AddTransient<QueryFactory>(e =>
      {
        var connection = new MySqlConnection(
          "Host=localhost;Port=60339;User=dboFinance;Password=mdn9VBYldGcmLo01lt5Y3lpQqeE=;Database=dboFinance;SslMode=None"
        );

        var compiler = new MySqlCompiler();
        return new QueryFactory(connection, compiler);
      });

      services.AddControllers()
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
          options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        });
    }

    private static void ConfigureHandlerModules(IServiceCollection services)
    {
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

      // Uncomment if authorization middleware is needed
      // app.UseAuthorization();

      app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    }
  }

  public static class Log4netExtensions
  {
    public static void AddLog4Net(this ILoggingBuilder loggingBuilder)
    {
      var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

      loggingBuilder.AddProvider(new Log4NetLoggerProvider());
    }
  }

  public class Log4NetLoggerProvider : ILoggerProvider
  {
    public ILogger CreateLogger(string categoryName)
    {
      return new Log4NetLogger(categoryName);
    }

    public void Dispose()
    {
    }
  }

  public class Log4NetLogger : ILogger
  {
    private readonly ILog _logger;

    public Log4NetLogger(string categoryName)
    {
      _logger = LogManager.GetLogger(categoryName);
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
      Func<TState, Exception, string> formatter)
    {
      var logMessage = formatter(state, exception);
      switch (logLevel)
      {
        case LogLevel.Trace:
        case LogLevel.Debug:
          _logger.Debug(logMessage);
          break;
        case LogLevel.Information:
          _logger.Info(logMessage);
          break;
        case LogLevel.Warning:
          _logger.Warn(logMessage);
          break;
        case LogLevel.Error:
          _logger.Error(logMessage);
          break;
        case LogLevel.Critical:
          _logger.Fatal(logMessage);
          break;
        case LogLevel.None:
          break;
      }
    }
  }
}
