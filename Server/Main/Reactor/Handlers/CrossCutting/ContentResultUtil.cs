using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using static System.Text.Json.Serialization.JsonIgnoreCondition;

namespace Server.Main.Reactor.Handlers.CrossCutting;

public class ContentResultUtil
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(ContentResultUtil));

  public static IObservable<JsonResult> Render<T>(T resultInstance)
  {
    var jsonOptions = new JsonSerializerOptions
    {
      DefaultIgnoreCondition = WhenWritingNull,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
      PropertyNameCaseInsensitive = true
    };
    return Observable.Return(new JsonResult(resultInstance, jsonOptions));
  }

  public static IObservable<IActionResult> Throw(Exception exception, int statusCode)
  {
    Logger.Error($"ContentResultUtil@Throw result :: {exception}");
    return Observable.Return(new StatusCodeResult(statusCode));
  }
}
