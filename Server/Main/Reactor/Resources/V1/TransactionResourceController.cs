using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.Business.Transactions;
using Server.Main.Reactor.Models.Dto.Transactions;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Resources.V1;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("v1/transactionResource")]
[Produces("application/json")]
public class TransactionResourceController : ControllerBase
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(TransactionResourceController));

  private readonly SettlementHandler _settlementHandler;
  private readonly CreateRefundTransactionHandler _createRefundTransactionHandler;
  private readonly CreateAdhocTransactionHandler _createAdhocTransactionHandler;
  private readonly QueryTransactionHistoryHandler _queryTransactionHistoryHandler;

  public TransactionResourceController(SettlementHandler settlementHandler, CreateRefundTransactionHandler createRefundTransactionHandler, CreateAdhocTransactionHandler createAdhocTransactionHandler, QueryTransactionHistoryHandler queryTransactionHistoryHandler)
  {
    _settlementHandler = settlementHandler;
    _createRefundTransactionHandler = createRefundTransactionHandler;
    _createAdhocTransactionHandler = createAdhocTransactionHandler;
    _queryTransactionHistoryHandler = queryTransactionHistoryHandler;
  }

  [HttpPost]
  [Route("settlement")]
  public Task<IActionResult> CreateSettlement([FromForm] UpdateTransactionDto dto)
  {
    Logger.Info("TransactionResourceController@CreateSettlement initiated...");
    return _settlementHandler.Handle(dto)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

  [HttpPost]
  [Route("requestRefund")]
  public Task<IActionResult> RequestRefund([FromBody] UpdateTransactionDto dto)
  {
    Logger.Info("TransactionResourceController@RequestRefund initiated...");
    return _createRefundTransactionHandler.Handle(dto)
      .Catch<IActionResult, Exception>(ex => ContentResultUtil.Throw(ex, StatusCodes.Status422UnprocessableEntity))
      .ToTask();
  }

  [HttpPost]
  [Route("adhoc")]
  public Task<IActionResult> CreateAdhocTransaction([FromBody] CreateTransactionDto dto)
  {
    Logger.Info("TransactionResourceController@CreateAdhocTransaction initiated...");
    return _createAdhocTransactionHandler.Handle(dto)
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

