using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Src.Main.Reactor.Models.Dto;

namespace Src.Main.Reactor.Clients;

public class FxHttpClient
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(FxHttpClient));

  private readonly HttpClient _httpClient;

  public FxHttpClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public IObservable<ExchangeRatesDto?> QueryExternalPartyExchangeRates()
  {
    Logger.Info("FxHttpClient@QueryExternalPartyExchangeRates initiated...");
    return Observable
      .FromAsync(() => _httpClient.GetAsync("https://latest.currency-api.pages.dev/v1/currencies/eur.json"))
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(response => Logger.Info($"QueryCurrenciesHandler@QueryExternalPartyExchangeRates http result :: {response.IsSuccessStatusCode}"))
      .SelectMany(UnmarshallClientResult)
      .Catch<ExchangeRatesDto?, Exception>(ex => Observable.Empty<ExchangeRatesDto?>());
  }

  private static IObservable<ExchangeRatesDto?> UnmarshallClientResult(HttpResponseMessage response)
  {
    return Observable.FromAsync(async () =>
    {
      var content = await response.Content.ReadAsStringAsync();
      Logger.Info($"FxHttpClient@UnmarshallClientResult response payload :: {content}");
      return JsonSerializer.Deserialize<ExchangeRatesDto?>(content, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });
    });
  }
}
