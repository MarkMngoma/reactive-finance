using log4net;

namespace Server.Main.Reactor.Middleware;

public class NDCRequestLoggingMiddleware
{
  private readonly RequestDelegate _next;

  public NDCRequestLoggingMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext httpContext)
  {
    var requestId = Guid.NewGuid().ToString();
    GlobalContext.Properties["NDC"] = requestId;
    var logger = LogManager.GetLogger(typeof(NDCRequestLoggingMiddleware));
    await _next(httpContext);
  }
}
