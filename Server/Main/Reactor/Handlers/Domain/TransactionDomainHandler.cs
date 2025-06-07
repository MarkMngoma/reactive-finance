using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Server.Main.Reactor.Builders;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;
using Server.Main.Reactor.Models.Request.Transactions;
using SqlKata.Execution;

using static Server.Main.Reactor.Builders.Tables.Generated.TransactionsTable;

namespace Server.Main.Reactor.Handlers.Domain;

public class TransactionDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public TransactionDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<int> InsertTransaction(CreateTransactionRequest request, ulong subscriptionId)
  {
    return Observable.FromAsync(() => _queryFactory
      .Query(TableName)
      .InsertAsync(new TransactionRecordBuilder()
        .WithAmount(request.Amount)
        .WithType(request.Type)
        .WithCurrency("ZAR")
        .WithStatus(Models.Enums.TransactionStatus.Pending.ToString())
        .WithTransactionDate(DateTime.UtcNow)
        .WithDescription(request.Description ?? "Subscription Payment")
        .WithSubscriptionId(subscriptionId)
        .WithCreatedAt(DateTime.UtcNow)
        .Build())
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> UpdateTransaction(UpdateTransactionRequest request)
  {
    return SelectTransactionUsingTransactionId(request.TransactionId)
      .Do(existingTransaction =>
      {
        if (existingTransaction.TransactionId == null)
        {
          throw new StandardException("Transaction not found.", StatusCodes.Status404NotFound);
        }
      })
      .Select(existingRecord =>
      {
        existingRecord.Status = request.Status;
        existingRecord.UpdatedAt = DateTime.UtcNow;
        return existingRecord;
      })
      .SelectMany(record => _queryFactory.Query(TableName).UpdateAsync(record))
      .SelectMany(_ => SelectTransactionUsingTransactionId(request.TransactionId))
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<IEnumerable<TransactionsDto>> SelectTransactionsUsingId(ulong id)
  {
    return Observable.FromAsync(() =>
       _queryFactory.Query(TableName)
         .Select(TransactionId, TransactionsTable.Type, Currency, Amount, Description, Status, AuthorizationId, SubscriptionId, TransactionDate)
         .Where(Id, id)
         .OrWhere(AuthorizationId, id)
         .Limit(2)
         .GetAsync<TransactionsDto>()
     )
     .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> SelectSettlementTransactionUsingId(ulong id)
  {
    return Observable.FromAsync(() =>
       _queryFactory.Query(TableName)
         .Select(TransactionId, TransactionsTable.Type, Currency, Amount, Description, Status, AuthorizationId, SubscriptionId, TransactionDate)
         .Where(AuthorizationId, id)
         .Limit(1)
         .FirstOrDefaultAsync<TransactionsDto>()
     )
     .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<IEnumerable<TransactionsDto>> SelectTransactionsUsingTransactionId(string? id)
  {
    return Observable.FromAsync(() =>
       _queryFactory.Query(TableName)
         .Select(TransactionId, TransactionsTable.Type, Currency, Amount, Description, Status, AuthorizationId, SubscriptionId, TransactionDate)
         .Where(TransactionId, id)
         .GetAsync<TransactionsDto>()
     )
     .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> SelectTransactionUsingTransactionId(string? id)
  {
    return Observable.FromAsync(() =>
       _queryFactory.Query(TableName)
         .Select(TransactionId, TransactionsTable.Type, Currency, Amount, Description, Status, AuthorizationId, SubscriptionId, TransactionDate)
         .Where(TransactionId, id)
         .Limit(1)
         .FirstOrDefaultAsync<TransactionsDto>()
     )
     .SubscribeOn(TaskPoolScheduler.Default);
  }
}
