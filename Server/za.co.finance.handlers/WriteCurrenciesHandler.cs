using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using server.za.co.finance.models.dto;
using SqlKata.Execution;

namespace server.za.co.finance.handlers;

public class WriteCurrenciesHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly string _tableName = "CURRENCIES";

  private readonly QueryFactory _commandQueryFactory;
  private readonly QueryCurrenciesHandler _handler;

  public WriteCurrenciesHandler(QueryFactory commandQueryFactory, QueryCurrenciesHandler handler)
  {
    _commandQueryFactory = commandQueryFactory;
    _handler = handler;
  }

  public IObservable<JsonResult> HandleCurrencyCreation(CurrencyDto currencyDto)
  {
    var insertData = new
    {
      CURRENCY_ID = currencyDto.CurrencyId,
      CURRENCY_CODE = currencyDto.CurrencyCode,
      CURRENCY_SYMBOL = currencyDto.CurrencySymbol,
      CURRENCY_FLAG = currencyDto.CurrencyFlag,
      CURRENCY_NAME = currencyDto.CurrencyName,
      ARCHIVED = 0,
      CREATED_AT = DateTimeOffset.Now,
      CREATED_BY = 1
    };

    Logger.Info($"QueryCurrenciesHandler@HandleCurrencyCreation initiated for :: {insertData}");
    return Observable.FromAsync(() => _commandQueryFactory.Query(_tableName).InsertAsync(insertData))
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Info($"HomeController@CreateNewCurrency http result :: {dataResult}"))
      .SelectMany(dataResult => _handler.FetchCurrencyUsingCode(currencyDto.CurrencyCode))
      .Select(dataResult => new JsonResult(dataResult));
  }
}
