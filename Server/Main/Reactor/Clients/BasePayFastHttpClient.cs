using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using log4net;

namespace Server.Main.Reactor.Clients;

public abstract class BasePayFastHttpClient
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(BasePayFastHttpClient));

  protected readonly HttpClient _httpClient;

  protected BasePayFastHttpClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
  {
    try
    {
      Logger.Debug($"Sending request to {request.RequestUri}");
      var response = await _httpClient.SendAsync(request);
      Logger.Debug($"Received response with status code {response.StatusCode} from {request.RequestUri}");
      return response.EnsureSuccessStatusCode();
    }
    catch (Exception ex)
    {
      Logger.Error($"Error sending request to {request.RequestUri}: {ex.Message}", ex);
      throw;
    }
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
