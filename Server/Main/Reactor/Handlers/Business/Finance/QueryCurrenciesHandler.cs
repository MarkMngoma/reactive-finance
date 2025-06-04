using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Clients;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class QueryCurrenciesHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly FxHttpClient _fxHttpClient;
  private readonly CurrencyDao _currencyDao;

  public QueryCurrenciesHandler(FxHttpClient fxHttpClient, CurrencyDao currencyDao)
  {
    _fxHttpClient = fxHttpClient;
    _currencyDao = currencyDao;
  }

  public IObservable<JsonResult> QueryPartyExchangeRates()
  {
    Logger.Info("QueryCurrenciesHandler@QueryPartyExchangeRates initiated...");
    return _fxHttpClient.QueryExternalPartyExchangeRates()
      .Do(dto => Logger.Debug($"QueryCurrenciesHandler@QueryPartyExchangeRates preparing response :: {dto}"))
      .SelectMany(ContentResultUtil.Render)
      .SubscribeOn(new EventLoopScheduler());
  }

  public IObservable<JsonResult> QueryCollectiveCurrencies()
  {
    Logger.Info("QueryCurrenciesHandler@QueryCollectiveCurrencies initiated...");
    return _currencyDao.SelectEnumerableCurrencies()
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Debug($"QueryCurrenciesHandler@QueryCollectiveCurrencies domain result :: {dataResult.ToList().Count}"))
      .SelectMany(ContentResultUtil.Render)
      .Finally(() => Logger.Info($"SubscribedOn: {Thread.CurrentThread.Name}"))
      .SubscribeOn(new EventLoopScheduler());
  }

  public IObservable<JsonResult> QueryCurrencyUsingCurrencyCode(string currencyCode)
  {
    Logger.Info($"QueryCurrenciesHandler@QueryCurrencyUsingCurrencyCode initiated for :: {currencyCode}");
    return _currencyDao.SelectCurrencyUsingCode(currencyCode)
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Debug($"QueryCurrenciesHandler@QueryCurrencyUsingCurrencyCode domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .SelectMany(ContentResultUtil.Render)
      .Finally(() => Logger.Info($"SubscribedOn: {Thread.CurrentThread.Name}"))
      .SubscribeOn(new EventLoopScheduler());
  }

}
