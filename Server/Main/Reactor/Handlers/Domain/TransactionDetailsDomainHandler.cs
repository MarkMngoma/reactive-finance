using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using SqlKata.Execution;
using static Server.Main.Reactor.Builders.Tables.Generated.TransactionDetailsTable;

namespace Server.Main.Reactor.Handlers.Domain;

public class TransactionDetailsDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public TransactionDetailsDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<TransactionDetailsDto> SelectTransactionsUsingTransactionId(string? id)
  {
    return Observable.FromAsync(() =>
       _queryFactory.Query(TableName)
         .Select(Id, TransactionId, EventType, Status, ReceivedAt, ProcessedAt)
         .Join(TransactionsTable.TableName, TransactionsTable.Id, TransactionId)
         .Select(
            TransactionsTable.TableName,
            TransactionsTable.TransactionId,
            TransactionsTable.SubscriptionId,
            TransactionsTable.AuthorizationId,
            TransactionsTable.Type,
            TransactionsTable.Status,
            TransactionsTable.Currency,
            TransactionsTable.Amount,
            TransactionsTable.TransactionDate,
            TransactionsTable.Description
        )
         .Where(TransactionId, id)
         .FirstOrDefaultAsync<TransactionDetailsDto>()
     )
     .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> UpdateTransactionDetails(TransactionsDto transaction)
  {
    throw new NotImplementedException();
  }
}
