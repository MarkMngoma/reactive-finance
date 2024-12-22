using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using Src.Main.Reactor.Handlers.CrossCutting;
using Src.Main.Reactor.Models.Dto;

namespace Src.Main.Reactor.Handlers.Business;

public class WriteCurrenciesHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private const string TableName = "CURRENCIES";

  private readonly QueryFactory _commandQueryFactory;
  private readonly QueryCurrenciesHandler _handler;
  private readonly ContentResultHandler _contentResultHandler;

  public WriteCurrenciesHandler(QueryFactory commandQueryFactory, QueryCurrenciesHandler handler, ContentResultHandler contentResultHandler)
  {
    _commandQueryFactory = commandQueryFactory;
    _handler = handler;
    _contentResultHandler = contentResultHandler;
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
    return Observable.FromAsync(() => _commandQueryFactory.Query(TableName).InsertAsync(insertData))
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Info($"HomeController@CreateNewCurrency http result :: {dataResult}"))
      .SelectMany(dataResult => _handler.FetchCurrencyUsingCode(currencyDto.CurrencyCode))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }
}
