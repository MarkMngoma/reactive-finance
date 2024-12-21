using log4net;

namespace server.za.co.finance.middleware;

public class LoggingMiddleware
{
  private readonly RequestDelegate _next;

  public LoggingMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext httpContext)
  {
    var requestId = Guid.NewGuid().ToString();
    GlobalContext.Properties["NDC"] = requestId;
    var logger = LogManager.GetLogger(typeof(LoggingMiddleware));

    var httpMethod = httpContext.Request.Method;
    var requestPath = httpContext.Request.Path;
    var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    try
    {
      logger.Info($"---> Received {httpMethod} {requestPath} from {clientIp} - id={requestId}");
      await _next(httpContext);
    }
    finally
    {
      stopwatch.Stop();
      var responseStatusCode = httpContext.Response.StatusCode;
      var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

      logger.Info($"---> {httpMethod} {requestPath}: [{responseStatusCode}] - id={requestId} - {elapsedMilliseconds}ms");
    }
  }
}
