using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Server.UnitTests.Main;

public class JsonRendererUtil
{
  public static T ConvertAndRender<T>(JsonResult result)
  {
    var jsonString = JsonConvert.SerializeObject(result.Value);
    return JsonConvert.DeserializeObject<T>(jsonString) ?? throw new InvalidOperationException();
  }
}
