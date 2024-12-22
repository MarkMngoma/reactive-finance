using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using Src.Main.Reactor.Clients;
using Src.Main.Reactor.Handlers.CrossCutting;

namespace Src.Main.Reactor.Handlers.Business;

public class QueryCurrenciesHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private const string TableName = "CURRENCIES";

  private readonly FxHttpClient _fxHttpClient;
  private readonly QueryFactory _queryFactory;
  private readonly ContentResultHandler _contentResultHandler;

  public QueryCurrenciesHandler(FxHttpClient fxHttpClient, QueryFactory queryFactory, ContentResultHandler contentResultHandler)
  {
    _fxHttpClient = fxHttpClient;
    _queryFactory = queryFactory;
    _contentResultHandler = contentResultHandler;
  }

  public IObservable<JsonResult> QueryPartyExchangeRates()
  {
    Logger.Info("QueryCurrenciesHandler@QueryPartyExchangeRates initiated...");
    return _fxHttpClient.QueryExternalPartyExchangeRates()
      .Do(dto => Logger.Info($"QueryCurrenciesHandler@QueryPartyExchangeRates preparing response :: {dto}"))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }

  public IObservable<JsonResult> QueryCollectiveCurrencies()
  {
    Logger.Info("QueryCurrenciesHandler@QueryCollectiveCurrencies initiated...");
    return Observable.FromAsync(() => _queryFactory.Query(TableName)
        .Select("ID", "CURRENCY_ID", "CURRENCY_CODE", "CURRENCY_NAME", "CURRENCY_SYMBOL", "CURRENCY_FLAG")
        .GetAsync()
      )
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Info($"QueryCurrenciesHandler@QueryCollectiveCurrencies domain result :: {dataResult.ToString()}"))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }

  public IObservable<JsonResult> QueryCurrencyUsingCurrencyCode(string currencyCode)
  {
    Logger.Info($"QueryCurrenciesHandler@QueryCollectiveCurrencies initiated for :: {currencyCode}");
    return FetchCurrencyUsingCode(currencyCode)
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Info($"QueryCurrenciesHandler@QueryCollectiveCurrencies domain result :: {dataResult.ToString()}"))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }

  public IObservable<IEnumerable<dynamic>> FetchCurrencyUsingCode(string currencyCode)
  {
    return Observable.FromAsync(() => _queryFactory.Query(TableName)
      .Select("ID", "CURRENCY_ID", "CURRENCY_CODE", "CURRENCY_NAME", "CURRENCY_SYMBOL", "CURRENCY_FLAG")
      .Where("CURRENCY_CODE", currencyCode)
      .Limit(1)
      .GetAsync()
    );
  }
}
