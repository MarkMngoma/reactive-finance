using System.Reactive.Concurrency;
using System.Reactive.Linq;
using log4net;
using Server.Main.Reactor.Models.Request;

namespace Server.Main.Reactor.Clients;

public class FxHttpClient : BaseHttpClient
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(FxHttpClient));


  public FxHttpClient(HttpClient httpClient) : base(httpClient)
  {
  }

  public IObservable<ExchangeRatesRequest?> QueryExternalPartyExchangeRates()
  {
    Logger.Info("FxHttpClient@QueryExternalPartyExchangeRates initiated...");
    return Observable
      .FromAsync(() => _httpClient.GetAsync("https://latest.currency-api.pages.dev/v1/currencies/eur.json"))
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(response => Logger.Info($"QueryCurrenciesHandler@QueryExternalPartyExchangeRates http result :: {response.IsSuccessStatusCode}"))
      .SelectMany(Unmarshall<ExchangeRatesRequest>)
      .Catch<ExchangeRatesRequest?, Exception>(ex => Observable.Empty<ExchangeRatesRequest?>())
      .SubscribeOn(CurrentThreadScheduler.Instance);
  }
}
