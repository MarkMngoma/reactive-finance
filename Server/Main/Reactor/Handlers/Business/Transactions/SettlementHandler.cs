using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Enums;
using Server.Main.Reactor.Models.Request.Transactions;
using Server.Main.Reactor.Models.Response;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class SettlementHandler : IHandler<UpdateTransactionRequest>
{

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

  public IObservable<JsonResult> Handle(UpdateTransactionRequest request)
  {
    return HandleValidation(request)
      .SelectMany(_transactionDomainHandler.UpdateTransaction)
      .Select(_ => new JsonResult(new BasicResponse(true)))
      .SubscribeOn(new EventLoopScheduler())
      .Do(_ => Console.WriteLine($"SettlementHandler@Handle completed for TransactionId: {request.TransactionId}"))
      .Finally(() => Console.WriteLine($"SettlementHandler@Handle completed for TransactionId: {request.TransactionId}"));
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
