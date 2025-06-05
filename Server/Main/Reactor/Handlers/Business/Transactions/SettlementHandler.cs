using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.CrossCutting.Utils;
using Server.Main.Reactor.Models.Enums;
using Server.Main.Reactor.Models.Request.Transactions;
using Server.Main.Reactor.Models.Response;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class SettlementHandler : Handler<UpdateTransactionRequest>
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(SettlementHandler));
  private readonly TransactionDomainHandler _transactionDomainHandler;
  private readonly TransactionDetailsDomainHandler _transactionDetailsDomainHandler;

  private readonly List<string> _validStatusList;

  public SettlementHandler(TransactionDomainHandler transactionDomainHandler, TransactionDetailsDomainHandler transactionDetailsDomainHandler)
  {
    _transactionDomainHandler = transactionDomainHandler;
    _transactionDetailsDomainHandler = transactionDetailsDomainHandler;
    _validStatusList =
    [
      TransactionStatus.Pending.ToString(),
      TransactionStatus.Authorized.ToString()
    ];
  }

  public override IObservable<JsonResult> Handle(UpdateTransactionRequest request)
  {
    return ExecCompute(request)
      .SelectMany(HandleValidation)
      .SelectMany(HandleExternalConfirmation)
      .SelectMany(CompleteSettlement)
      .SelectMany(HandlePartyNotifications)
      .Select(_ => ContentResultUtil.Render(new BasicResponse(true)))
      .Do(_ => Logger.Info($"SettlementHandler@Handle completed for TransactionId: {request.TransactionId}"));
  }

  public IObservable<UpdateTransactionRequest> HandleExternalConfirmation(UpdateTransactionRequest request)
  {

    // TODO: Implement external confirmation logic to call PayFast API transaction confirmation.
    Logger.Debug($"SettlementHandler@HandleExternalConfirmation initiated for TransactionId: {request.TransactionId}");
    return null;
  }

  public IObservable<TransactionsDto> HandlePartyNotifications(TransactionsDto request)
  {
    // TODO: Implement party notification logic, e.g., sending emails or notifications to parties involved in the transaction.
    // This could involve calling a notification service or sending messages to a message queue.
    // For now, we return null to indicate that this part is not yet implemented.
    // Example: return _notificationService.SendTransactionNotification(request);
    return null;
  }

  public IObservable<TransactionsDto> CompleteSettlement(UpdateTransactionRequest request)
  {
    return _transactionDomainHandler.UpdateTransaction(request)
      .SelectMany(_transactionDetailsDomainHandler.UpdateTransactionDetails);
  }

  private IObservable<UpdateTransactionRequest> HandleValidation(UpdateTransactionRequest request)
  {
    return _transactionDomainHandler
    .SelectTransactionById(request.TransactionId)
    .Select(transaction =>
    {
      if (transaction == null)
      {
        throw new InvalidOperationException("Transaction not found.");
      }

      if (_validStatusList.Contains(request.Status) == false)
      {
        throw new InvalidOperationException($"Invalid transaction status: {request.Status}.");
      }
      return request;
    });
  }
}
