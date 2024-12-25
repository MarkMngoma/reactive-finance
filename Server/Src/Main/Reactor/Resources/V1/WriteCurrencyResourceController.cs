using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Src.Main.Reactor.Handlers.Business;
using Src.Main.Reactor.Handlers.CrossCutting;
using Src.Main.Reactor.Models.Dto;

namespace Src.Main.Reactor.Resources.V1;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public class WriteCurrencyResourceController : ControllerBase
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(WriteCurrencyResourceController));

  private readonly ThrowableHandler _throwableHandler;
  private readonly WriteCurrenciesHandler _handler;
  private readonly WriteBatchCurrenciesHandler _handlerBatch;

  public WriteCurrencyResourceController(ThrowableHandler throwableHandler, WriteCurrenciesHandler handler, WriteBatchCurrenciesHandler handlerBatch)
  {
    _throwableHandler = throwableHandler;
    _handler = handler;
    _handlerBatch = handlerBatch;
  }

  [HttpPost]
  public Task<IActionResult> CreateNewCurrency([FromBody] CurrencyDto currencyDto)
  {
    Logger.Info("WriteCurrencyResourceController@CreateNewCurrency initiated...");
    return _handler.Handle(currencyDto)
      .Catch<IActionResult, Exception>(ex => _throwableHandler.Handle(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

  [HttpPost]
  [Route("Batch")]
  public Task<IActionResult> CreateNewBatchCurrencies([FromBody] BatchCurrencyDto batchCurrencyDto)
  {
    Logger.Info("WriteCurrencyResourceController@CreateNewBatchCurrencies initiated...");
    return _handlerBatch.Handle(batchCurrencyDto)
      .Catch<IActionResult, Exception>(ex => _throwableHandler.Handle(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

}
