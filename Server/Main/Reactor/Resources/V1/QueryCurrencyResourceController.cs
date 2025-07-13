using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.Business.Finance;
using Server.Main.Reactor.Models.Dto.Queries;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Resources.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/queryCurrencyResource")]
[Produces("application/json")]
public class QueryCurrencyResourceController: ControllerBase
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrencyResourceController));

  private readonly QueryCurrenciesHandler _queryCurrenciesHandler;
  private readonly QueryExchangeRateHandler _queryExchangeRateHandler;

  public QueryCurrencyResourceController(QueryCurrenciesHandler queryCurrenciesHandler, QueryExchangeRateHandler queryExchangeRateHandler)
  {
    _queryCurrenciesHandler = queryCurrenciesHandler;
    _queryExchangeRateHandler = queryExchangeRateHandler;
  }

  [HttpGet]
  public Task<IActionResult> GetSupportedCurrencies()
  {
    Logger.Info("QueryCurrencyResourceController@GetSupportedCurrencies initiated...");
    return _queryCurrenciesHandler.HandleCurrencyListQuery()
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }

  [HttpGet]
  [Route("{currencyCode}")]
  public Task<IActionResult> GetSupportedCurrencyCodes(string currencyCode)
  {
    Logger.Info("QueryCurrencyResourceController@GetSupportedCurrencyCodes initiated...");
    var request = new QueryCurrencyDto
    {
      CurrencyCode = currencyCode
    };
    return _queryCurrenciesHandler.Handle(request)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }

  [HttpGet]
  [Route("exchangeRates")]
  public Task<IActionResult> GetExchangeRates(string quoteCurrency)
  {
    Logger.Info("QueryCurrencyResourceController@GetExchangeRates initiated...");
    var request = new QueryCurrencyDto
    {
      QuoteCurrencyCode = quoteCurrency
    };
    return _queryExchangeRateHandler.Handle(request)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }
}
