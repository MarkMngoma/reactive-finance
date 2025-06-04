using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Server.Main.Reactor.Builders;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Models.Request.Transactions;
using SqlKata.Execution;

namespace Server.Main.Reactor.Domain;

public class TransactionDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public TransactionDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<int> InsertTransaction(CreateTransactionRequest request)
  {
    var record = new TransactionRecordBuilder()
      .WithAmount(request.Amount)
      .WithType(request.Type)
      .WithCurrency("ZAR")
      .WithStatus(Models.Enums.TransactionStatus.Pending.ToString())
      .WithTransactionDate(DateTime.UtcNow)
      .WithDescription(request.Description ?? "Subscription Payment")
      //.WithSubscriptionId(request.SubscriptionId)
      .WithCreatedAt(DateTime.UtcNow)
      .Build();
    return Observable.FromAsync(() => _queryFactory
      .Query(TransactionsTable.TableName)
      .InsertAsync(record))
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<int> UpdateTransaction(UpdateTransactionRequest request)
  {
    return SelectTransactionById(request.TransactionId)
      .Do(existingTransaction =>
      {
        if (existingTransaction.TransactionId == null)
        {
          throw new InvalidOperationException("Transaction not found.");
        }
      })
      .Select(existingRecord =>
      {
        existingRecord.Status = request.Status;
        existingRecord.UpdatedAt = DateTime.UtcNow;
        return existingRecord;
      })
      .SelectMany(record => _queryFactory
        .Query(TransactionsTable.TableName)
        .UpdateAsync(record)
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> SelectTransactionById(string? searchId)
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TransactionsTable.TableName)
          .Select(
            TransactionsTable.TransactionId,
            TransactionsTable.SubscriptionId,
            TransactionsTable.Type,
            TransactionsTable.Amount,
            TransactionsTable.Currency,
            TransactionsTable.Status,
            TransactionsTable.AuthorizationId,
            TransactionsTable.TransactionDate,
            TransactionsTable.Description
          )
          .Where(TransactionsTable.TransactionId, searchId)
          .OrWhere(TransactionsTable.SubscriptionId, searchId)
          .OrWhere(TransactionsTable.AuthorizationId, searchId)
          .OrWhere(TransactionsTable.Id, searchId)
          .FirstOrDefaultAsync<TransactionsDto>()
      )
      .DefaultIfEmpty(new TransactionsDto())
      .SubscribeOn(Scheduler.Default);
  }
}
