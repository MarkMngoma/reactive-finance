using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
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
    return Observable.Return(new ContentResult
    {
      Content = JsonSerializer.Serialize(new { error = exception.Message }),
      ContentType = "application/json",
      StatusCode = statusCode
    });
  }
}
