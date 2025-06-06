using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Configuration.Objects;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.Enums;
using Server.Main.Reactor.Models.Request.Transactions;
using Server.Main.Reactor.Models.Response;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class SettlementHandler : Handler<UpdateTransactionRequest>
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(SettlementHandler));
  private readonly TransactionDomainHandler _transactionDomainHandler;
  private readonly TransactionDetailsDomainHandler _transactionDetailsDomainHandler;
  private readonly TransactionConfig _transactionConfig;

  public SettlementHandler(TransactionDomainHandler transactionDomainHandler, TransactionDetailsDomainHandler transactionDetailsDomainHandler, TransactionConfig transactionConfig)
  {
    _transactionDomainHandler = transactionDomainHandler;
    _transactionDetailsDomainHandler = transactionDetailsDomainHandler;
    _transactionConfig = transactionConfig;
  }

  public override IObservable<JsonResult> Handle(UpdateTransactionRequest request)
  {
    return HandleComputeEvent(request)
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
    return Observable.Return(request);
  }

  public IObservable<TransactionsDto> HandlePartyNotifications(TransactionsDto request)
  {
    // TODO: Implement party notification logic, e.g., sending emails or notifications to parties involved in the transaction.
    // This could involve calling a notification service or sending messages to a message queue.
    // For now, we return null to indicate that this part is not yet implemented.
    // Example: return _notificationService.SendTransactionNotification(request);
    return Observable.Return(request);
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
    .Select(transaction => new TransactionsDto() { Status = TransactionStatus.Pending.ToString() })
    .Select(transaction =>
    {
      if (transaction == null)
      {
        throw new StandardException("Transaction not found.");
      }

      if (_transactionConfig.AllowedSettlementStatuses.Contains(request.Status) == false)
      {
        throw new StandardException($"Invalid transaction status: {request.Status}.");
      }
      return request;
    });
  }
}
