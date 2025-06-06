using System;
using Server.Main.Reactor.Builders.Tables.Generated.Models;

namespace Server.Main.Reactor.Handlers.Domain;

public class TransactionDetailsDomainHandler
{
  public IObservable<TransactionDetailsDto> SelectTransactionUsingId(string? transactionId)
  {
    throw new NotImplementedException();
  }

  public IObservable<TransactionsDto> UpdateTransactionDetails(TransactionsDto transaction)
  {
    throw new NotImplementedException();
  }
}
