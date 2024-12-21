using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using server.za.co.finance.handlers;
using server.za.co.finance.handlers.crosscutting;
using server.za.co.finance.models.dto;

namespace server.za.co.finance.resources.v1;

[ApiController]
[Route("v1/[controller]")]
public class WriteCurrencyResourceController : ControllerBase
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(WriteCurrencyResourceController));

  private readonly ThrowableHandler _throwableHandler;
  private readonly WriteCurrenciesHandler _handler;

  public WriteCurrencyResourceController(ThrowableHandler throwableHandler, WriteCurrenciesHandler handler)
  {
    _throwableHandler = throwableHandler;
    _handler = handler;
  }

  [HttpPost]
  public Task<IActionResult> CreateNewCurrency([FromBody] CurrencyDto currencyDto)
  {
    Logger.Info("WriteCurrencyResourceController@CreateNewCurrency initiated...");
    return _handler.HandleCurrencyCreation(currencyDto)
      .Catch<IActionResult, Exception>(ex => _throwableHandler.Handle(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

}
