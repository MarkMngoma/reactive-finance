using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;
using static System.Text.Json.Serialization.JsonIgnoreCondition;

namespace Server.Main.Reactor.Utils;

public class ContentResultUtil
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(ContentResultUtil));

  public static JsonResult Render<T>(T resultInstance)
  {
    var jsonOptions = new JsonSerializerOptions
    {
      DefaultIgnoreCondition = WhenWritingNull,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
      PropertyNameCaseInsensitive = true
    };
    return new JsonResult(resultInstance, jsonOptions)
    {
      StatusCode = StatusCodes.Status200OK
    };
  }

  public static IObservable<IActionResult> Throw(Exception exception, int statusCode)
{
    Logger.Error($"ContentResultUtil@Throw result :: {exception}");
    var message = exception is StandardException e ? e.Message : "Please try again later. If the problem persists, contact support.";
    var contentResult = new ContentResult
    {
        Content = JsonSerializer.Serialize(new { success = false, error = message }),
        ContentType = "application/json",
        StatusCode = exception is StandardException ? statusCode : StatusCodes.Status500InternalServerError
    };
    return Observable.Return(contentResult);
}
}
