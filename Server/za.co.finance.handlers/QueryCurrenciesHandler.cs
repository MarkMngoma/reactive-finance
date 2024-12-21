using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using server.za.co.bitbridge.clients;
using SqlKata.Execution;

namespace server.za.co.finance.handlers;

public class QueryCurrenciesHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly string _tableName = "CURRENCIES";

  private readonly FxHttpClient _fxHttpClient;
  private readonly QueryFactory _queryFactory;

  public QueryCurrenciesHandler(FxHttpClient fxHttpClient, QueryFactory queryFactory)
  {
    _fxHttpClient = fxHttpClient;
    _queryFactory = queryFactory;
  }

  public IObservable<JsonResult> QueryPartyExchangeRates()
  {
    Logger.Info("QueryCurrenciesHandler@QueryPartyExchangeRates initiated...");
    return _fxHttpClient.QueryExternalPartyExchangeRates()
      .Do(dto => Logger.Info($"QueryCurrenciesHandler@QueryPartyExchangeRates preparing response :: {dto}"))
      .Select(httpResult => new JsonResult(httpResult));
  }

  public IObservable<JsonResult> QueryCollectiveCurrencies()
  {
    Logger.Info("QueryCurrenciesHandler@QueryCollectiveCurrencies initiated...");
    return Observable.FromAsync(() => _queryFactory.Query(_tableName).GetAsync())
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Info($"QueryCurrenciesHandler@QueryCollectiveCurrencies domain result :: {dataResult.ToString()}"))
      .Select(dataResult => new JsonResult(dataResult));
  }

  public IObservable<JsonResult> QueryCurrencyUsingCurrencyCode(string currencyCode)
  {
    Logger.Info($"QueryCurrenciesHandler@QueryCollectiveCurrencies initiated for :: {currencyCode}");
    return FetchCurrencyUsingCode(currencyCode)
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Info($"QueryCurrenciesHandler@QueryCollectiveCurrencies domain result :: {dataResult.ToString()}"))
      .Select(dataResult => new JsonResult(dataResult));
  }

  public IObservable<IEnumerable<dynamic>> FetchCurrencyUsingCode(string currencyCode)
  {
    return Observable.FromAsync(() => _queryFactory.Query(_tableName)
      .Where("CURRENCY_CODE", currencyCode)
      .Limit(1)
      .GetAsync()
    );
  }
}
