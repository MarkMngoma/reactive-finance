using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using server.za.co.finance.handlers;
using server.za.co.finance.handlers.crosscutting;

namespace server.za.co.finance.resources.v1;

[ApiController]
[Route("v1/[controller]")]
public class QueryCurrencyResourceController: ControllerBase
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrencyResourceController));

  private readonly ThrowableHandler _throwableHandler;
  private readonly QueryCurrenciesHandler _queryCurrenciesHandler;

  public QueryCurrencyResourceController(ThrowableHandler throwableHandler, QueryCurrenciesHandler queryCurrenciesHandler)
  {
    _throwableHandler = throwableHandler;
    _queryCurrenciesHandler = queryCurrenciesHandler;
  }

  [HttpGet]
  public Task<IActionResult> GetSupportedCurrencies()
  {
    Logger.Info("QueryCurrencyResourceController@GetSupportedCurrencies initiated...");
    return _queryCurrenciesHandler.QueryCollectiveCurrencies()
    .Catch<IActionResult, Exception>(ex => _throwableHandler.Handle(ex, StatusCodes.Status404NotFound))
    .ToTask();
  }

  [HttpGet]
  [Route("{currencyCode}")]
  public Task<IActionResult> GetSupportedCurrencyCodes(string currencyCode)
  {
    Logger.Info("QueryCurrencyResourceController@GetSupportedCurrencyCodes initiated...");
    return _queryCurrenciesHandler.QueryCurrencyUsingCurrencyCode(currencyCode)
    .Catch<IActionResult, Exception>(ex => _throwableHandler.Handle(ex, StatusCodes.Status404NotFound))
    .ToTask();
  }

  [HttpGet]
  [Route("exchanges")]
  public Task<IActionResult> GetExchangeRates()
  {
    Logger.Info("QueryCurrencyResourceController@GetExchangeRates initiated...");
    return _queryCurrenciesHandler.QueryPartyExchangeRates()
      .Catch<IActionResult, Exception>(ex => _throwableHandler.Handle(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }
}
