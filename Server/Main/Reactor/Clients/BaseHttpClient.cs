using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using log4net;

namespace Server.Main.Reactor.Clients;

public abstract class BaseHttpClient
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseHttpClient));

  protected readonly HttpClient _httpClient;

  protected BaseHttpClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  protected IObservable<HttpResponseMessage> SendJsonAsync(HttpRequestMessage request)
  {
    return Observable
      .FromAsync(() => _httpClient.SendAsync(request))
      .Retry(3)
      .Do(response => Logger.Debug($"Received response with status code {response.StatusCode} from {request.RequestUri}"))
      .SubscribeOn(CurrentThreadScheduler.Instance);
  }

  protected IObservable<T?> Unmarshall<T>(HttpResponseMessage response)
  {
    return Observable.FromAsync(async () =>
    {
      var content = await response.Content.ReadAsStringAsync();
      Logger.Info($"BasePayFastHttpClient@UnmarshallClientResult<{typeof(T).Name}> response payload :: {content}");
      return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });
    });
  }

}
