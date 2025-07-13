using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Configuration.Objects;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.DTO;
using Server.Main.Reactor.Models.Dto.Transactions;
using Server.Main.Reactor.Models.Enums;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class SettlementHandler : Handler<UpdateTransactionDto>
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(SettlementHandler));
  private readonly ITransactionDomainHandler _transactionDomainHandler;
  private readonly ITransactionDetailsDomainHandler _transactionDetailsDomainHandler;
  private readonly TransactionConfig _transactionConfig;

  public SettlementHandler(ITransactionDomainHandler transactionDomainHandler, ITransactionDetailsDomainHandler transactionDetailsDomainHandler, TransactionConfig transactionConfig)
  {
    _transactionDomainHandler = transactionDomainHandler;
    _transactionDetailsDomainHandler = transactionDetailsDomainHandler;
    _transactionConfig = transactionConfig;
  }

  public override IObservable<JsonResult> Handle(UpdateTransactionDto dto)
  {
    return HandleComputeEvent(dto)
      .SelectMany(HandleValidation)
      .SelectMany(HandleExternalConfirmation)
      .SelectMany(CompleteSettlement)
      .SelectMany(HandlePartyNotifications)
      .Select(_ => ContentResultUtil.Render(new BasicResponseDto(true)))
      .Do(_ => Logger.Info($"SettlementHandler@Handle completed for TransactionId: {dto.TransactionId}"));
  }

  private IObservable<UpdateTransactionDto> HandleExternalConfirmation(UpdateTransactionDto dto)
  {

    // TODO: Implement external confirmation logic to call PayFast API transaction confirmation.
    Logger.Debug($"SettlementHandler@HandleExternalConfirmation initiated for TransactionId: {dto.TransactionId}");
    return Observable.Return(dto);
  }

  private IObservable<TransactionsDto> HandlePartyNotifications(TransactionsDto request)
  {
    // TODO: Implement party notification logic, e.g., sending emails or notifications to parties involved in the transaction.
    // This could involve calling a notification service or sending messages to a message queue.
    // For now, we return null to indicate that this part is not yet implemented.
    // Example: return _notificationService.SendTransactionNotification(request);
    return Observable.Return(request);
  }

  private IObservable<TransactionsDto> CompleteSettlement(UpdateTransactionDto dto)
  {
    return _transactionDomainHandler.UpdateTransaction(dto)
      .SelectMany(_transactionDetailsDomainHandler.UpdateTransactionDetails);
  }

  private IObservable<UpdateTransactionDto> HandleValidation(UpdateTransactionDto dto)
  {
    return _transactionDomainHandler
    .SelectTransactionUsingTransactionId(dto.TransactionId)
    .Select(transaction => new TransactionsDto() { Status = nameof(TransactionStatus.Pending) })
    .Select(transaction =>
    {
      if (transaction == null)
      {
        throw new StandardException("Transaction not found.");
      }

      if (_transactionConfig.AllowedSettlementStatuses.Contains(dto.Status) == false)
      {
        throw new StandardException($"Invalid transaction status: {dto.Status}.");
      }
      return dto;
    });
  }
}
