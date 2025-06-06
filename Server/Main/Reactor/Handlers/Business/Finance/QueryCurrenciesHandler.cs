using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Clients;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class QueryCurrenciesHandler : Handler<string>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly FxHttpClient _fxHttpClient;
  private readonly CurrencyDomainHandler _currencyDomainHandler;

  public QueryCurrenciesHandler(FxHttpClient fxHttpClient, CurrencyDomainHandler currencyDomainHandler)
  {
    _fxHttpClient = fxHttpClient;
    _currencyDomainHandler = currencyDomainHandler;
  }

  public override IObservable<JsonResult> Handle(string currencyCode)
  {
    Logger.Info($"QueryCurrenciesHandler@QueryCurrencyUsingCurrencyCode initiated for :: {currencyCode}");
    return HandleComputeEvent(currencyCode)
      .SelectMany(_currencyDomainHandler.SelectCurrencyUsingCode)
      .Do(dataResult => Logger.Debug($"QueryCurrenciesHandler@QueryCurrencyUsingCurrencyCode domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .Select(ContentResultUtil.Render);
  }

  public IObservable<JsonResult> HandlePartyExchangeRatesQuery()
  {
    Logger.Info("QueryCurrenciesHandler@HandlePartyExchangeRatesQuery initiated...");
    return _fxHttpClient.QueryExternalPartyExchangeRates()
      .Do(dto => Logger.Debug($"QueryCurrenciesHandler@HandlePartyExchangeRatesQuery preparing response :: {dto}"))
      .Select(ContentResultUtil.Render);
  }

  public IObservable<JsonResult> HandleCurrencyListQuery()
  {
    Logger.Info("QueryCurrenciesHandler@HandleCurrencyListQuery initiated...");
    return HandleComputeEvent(_currencyDomainHandler.SelectEnumerableCurrencies())
      .Do(dataResult => Logger.Debug($"QueryCurrenciesHandler@HandleCurrencyListQuery domain result :: {dataResult.ToList().Count}"))
      .Select(ContentResultUtil.Render);
  }

}
