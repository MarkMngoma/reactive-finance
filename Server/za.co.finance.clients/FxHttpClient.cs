using System.Reactive.Linq;
using log4net;
using Newtonsoft.Json;
using Server.Models.Dto;

namespace server.za.co.bitbridge.clients;

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
      .Do(response =>
        Logger.Info($"QueryCurrenciesHandler@QueryExternalPartyExchangeRates http result :: {response.IsSuccessStatusCode}"))
      .SelectMany(UnmarshallClientResult)
      .Catch<ExchangeRatesDto?, Exception>(ex => Observable.Empty<ExchangeRatesDto?>());
  }

  private IObservable<ExchangeRatesDto?> UnmarshallClientResult(HttpResponseMessage response)
  {
    return Observable.FromAsync(async () =>
    {
      var content = await response.Content.ReadAsStringAsync();
      Logger.Info($"FxHttpClient@UnmarshallClientResult response payload :: {content}");
      return JsonConvert.DeserializeObject<ExchangeRatesDto?>(content);
    });
  }
}
