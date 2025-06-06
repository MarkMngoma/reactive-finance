using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.Business.Transactions;
using Server.Main.Reactor.Models.Request.Transactions;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Resources.V1;

[ApiController]
[Route("v1/[controller]")]
[Produces("application/json")]
public class TransactionResourceController : ControllerBase
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(TransactionResourceController));

  private readonly SettlementHandler _settlementHandler;
  private readonly CreateRefundTransactionHandler _createRefundTransactionHandler;
  private readonly CreateAdhocTransactionHandler _createAdhocTransactionHandler;
  private readonly QueryTransactionHistoryHandler _queryTransactionHistoryHandler;

  public TransactionResourceController(SettlementHandler settlementHandler,
    CreateRefundTransactionHandler createRefundTransactionHandler,
    CreateAdhocTransactionHandler createAdhocTransactionHandler,
    QueryTransactionHistoryHandler queryTransactionHistoryHandler)
  {
    _settlementHandler = settlementHandler;
    _createRefundTransactionHandler = createRefundTransactionHandler;
    _createAdhocTransactionHandler = createAdhocTransactionHandler;
    _queryTransactionHistoryHandler = queryTransactionHistoryHandler;
  }

  [HttpPost]
  [Route("Settlement")]
  public Task<IActionResult> CreateSettlement([FromBody] UpdateTransactionRequest request)
  {
    Logger.Info("TransactionResourceController@CreateSettlement initiated...");
    return _settlementHandler.Handle(request)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

  [HttpPost]
  [Route("RequestRefund")]
  public Task<IActionResult> RequestRefund([FromBody] UpdateTransactionRequest request)
  {
    Logger.Info("TransactionResourceController@RequestRefund initiated...");
    return _createRefundTransactionHandler.Handle(request)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

  [HttpPost]
  [Route("Adhoc")]
  public Task<IActionResult> CreateAdhocTransaction([FromBody] CreateTransactionRequest request)
  {
    Logger.Info("TransactionResourceController@CreateAdhocTransaction initiated...");
    return _createAdhocTransactionHandler.Handle(request)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

  [HttpGet]
  public Task<IActionResult> GetTransactionHistory([FromRoute] string transactionId)
  {
    Logger.Info("TransactionResourceController@GetTransactionHistory initiated...");
    return _queryTransactionHistoryHandler.Handle(transactionId)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

}

