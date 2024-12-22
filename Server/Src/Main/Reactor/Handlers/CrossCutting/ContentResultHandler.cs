using System.Reactive.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using static System.Text.Json.Serialization.JsonIgnoreCondition;

namespace Src.Main.Reactor.Handlers.CrossCutting;

public class ContentResultHandler
{
  public IObservable<JsonResult> RenderContentResult<T>(T resultInstance)
  {
    var jsonOptions = new JsonSerializerOptions
    {
      DefaultIgnoreCondition = WhenWritingNull,
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
      DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
      PropertyNameCaseInsensitive = true
    };
    return Observable.Return(new JsonResult(resultInstance, jsonOptions));
  }
}
