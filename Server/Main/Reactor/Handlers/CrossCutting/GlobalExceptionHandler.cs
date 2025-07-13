using log4net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;

namespace Server.Main.Reactor.Handlers.CrossCutting;

public class GlobalExceptionHandler : IExceptionHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(GlobalExceptionHandler));
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    Logger.Error($"GlobalExceptionHandler@TryHandleAsync result :: {exception}");
    if (Environments.Development.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
    {
      Console.Error.WriteLine(exception);
    }
    var problemDetails = new ProblemDetails
    {
      Instance = httpContext.Request.Path
    };
    if (exception is StandardException e)
    {
      problemDetails.Title = e.Message;
      httpContext.Response.StatusCode = e.StatusCode;
    }
    else
    {
      problemDetails.Title = "An unexpected error occurred";
      problemDetails.Detail = "Please try again later. If the problem persists, contact support.";
      httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }
    problemDetails.Status = httpContext.Response.StatusCode;
    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
    return true;
  }
}
