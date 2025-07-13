using System.Reactive.Concurrency;
using System.Reactive.Linq;
using log4net;
using Server.Main.Reactor.Configuration.Objects;
using Server.Main.Reactor.Models.Dto.Currencies;

namespace Server.Main.Reactor.Clients;

public interface IFxHttpClient
{
  public IObservable<ExchangeRatesDto?> QueryExternalPartyExchangeRates(string? quoteCurrency);
}
public class FxHttpClient : BaseHttpClient, IFxHttpClient
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(FxHttpClient));

  private readonly ForexConfig _forexConfig;
  public FxHttpClient(HttpClient httpClient, ForexConfig forexConfig) : base(httpClient)
  {
    _forexConfig = forexConfig;
  }

  public IObservable<ExchangeRatesDto?> QueryExternalPartyExchangeRates(string? quoteCurrency)
  {
    Logger.Info("FxHttpClient@QueryExternalPartyExchangeRates initiated...");
    return Observable
      .FromAsync(() => _httpClient.GetAsync(BuildUri(quoteCurrency)))
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(response => Logger.Info($"QueryCurrenciesHandler@QueryExternalPartyExchangeRates http result :: {response.IsSuccessStatusCode}"))
      .SelectMany(Unmarshall<ExchangeRatesDto>)
      .Catch<ExchangeRatesDto?, Exception>(ex => Observable.Empty<ExchangeRatesDto?>())
      .SubscribeOn(CurrentThreadScheduler.Instance);
  }

  private Uri BuildUri(string? path)
  {
    UriBuilder uriBuilder = new(_forexConfig.ExchangeRatesApiUrl);
    uriBuilder.Path = uriBuilder.Path.TrimEnd('/') + "/" + path + ".json";
    return uriBuilder.Uri;
  }
}
