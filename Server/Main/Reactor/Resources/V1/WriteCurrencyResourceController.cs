using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.Business.Finance;
using Server.Main.Reactor.Models.Dto.Currencies;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Resources.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/writeCurrencyResource")]
[Produces("application/json")]
public class WriteCurrencyResourceController : ControllerBase
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(WriteCurrencyResourceController));

  private readonly WriteCurrenciesHandler _writeCurrenciesHandler;
  private readonly WriteBatchCurrenciesHandler _writeBatchCurrenciesHandler;

  public WriteCurrencyResourceController(WriteCurrenciesHandler writeCurrenciesHandler, WriteBatchCurrenciesHandler writeBatchCurrenciesHandler)
  {
    _writeCurrenciesHandler = writeCurrenciesHandler;
    _writeBatchCurrenciesHandler = writeBatchCurrenciesHandler;
  }

  [HttpPost]
  public Task<IActionResult> CreateNewCurrency([FromBody] CurrencyDto currencyDto)
  {
    Logger.Info("WriteCurrencyResourceController@CreateNewCurrency initiated...");
    return _writeCurrenciesHandler.Handle(currencyDto)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

  [HttpPost]
  [Route("batch")]
  public Task<IActionResult> CreateNewBatchCurrencies([FromBody] BatchCurrencyDto batchCurrencyDto)
  {
    Logger.Info("WriteCurrencyResourceController@CreateNewBatchCurrencies initiated...");
    return _writeBatchCurrenciesHandler.Handle(batchCurrencyDto)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

}
