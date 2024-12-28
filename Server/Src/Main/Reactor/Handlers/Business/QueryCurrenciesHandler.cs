using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using Src.Main.Reactor.Clients;
using Src.Main.Reactor.Handlers.CrossCutting;
using static Src.Main.Reactor.Builders.Tables.Generated.CurrenciesTable;

namespace Src.Main.Reactor.Handlers.Business;

public class QueryCurrenciesHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

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
      .Do(dto => Logger.Debug($"QueryCurrenciesHandler@QueryPartyExchangeRates preparing response :: {dto}"))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }

  public IObservable<JsonResult> QueryCollectiveCurrencies()
  {
    Logger.Info("QueryCurrenciesHandler@QueryCollectiveCurrencies initiated...");
    return Observable.FromAsync(() => _queryFactory.Query(TableName)
        .Select(Id, CurrencyId, CurrencyCode, CurrencyName, CurrencySymbol, CurrencyFlag)
        .GetAsync()
      )
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Debug($"QueryCurrenciesHandler@QueryCollectiveCurrencies domain result :: {dataResult.ToString()}"))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }

  public IObservable<JsonResult> QueryCurrencyUsingCurrencyCode(string currencyCode)
  {
    Logger.Info($"QueryCurrenciesHandler@QueryCurrencyUsingCurrencyCode initiated for :: {currencyCode}");
    return FetchCurrencyUsingCode(currencyCode)
      .Retry(3)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Debug($"QueryCurrenciesHandler@QueryCurrencyUsingCurrencyCode domain result :: {dataResult.ToString()}"))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }

  public IObservable<IEnumerable<dynamic>> FetchCurrencyUsingCode(string currencyCode)
  {
    return Observable.FromAsync(() => _queryFactory.Query(TableName)
      .Select(Id, CurrencyId, CurrencyCode, CurrencyName, CurrencySymbol, CurrencyFlag)
      .Where(CurrencyCode, currencyCode)
      .Limit(1)
      .GetAsync()
    );
  }
}
