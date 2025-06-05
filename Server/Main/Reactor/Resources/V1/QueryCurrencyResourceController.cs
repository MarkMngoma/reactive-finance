using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.Business.Finance;
using Server.Main.Reactor.Handlers.CrossCutting.Utils;

namespace Server.Main.Reactor.Resources.V1;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public class QueryCurrencyResourceController: ControllerBase
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrencyResourceController));

  private readonly QueryCurrenciesHandler _queryCurrenciesHandler;

  public QueryCurrencyResourceController(QueryCurrenciesHandler queryCurrenciesHandler)
  {
    _queryCurrenciesHandler = queryCurrenciesHandler;
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
    return _queryCurrenciesHandler.Handle(currencyCode)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }

  [HttpGet]
  [Route("exchanges")]
  public Task<IActionResult> GetExchangeRates()
  {
    Logger.Info("QueryCurrencyResourceController@GetExchangeRates initiated...");
    return _queryCurrenciesHandler.HandlePartyExchangeRatesQuery()
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status404NotFound))
      .ToTask();
  }
}
