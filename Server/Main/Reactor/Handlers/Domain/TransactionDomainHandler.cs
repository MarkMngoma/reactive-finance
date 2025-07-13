using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Server.Main.Reactor.Builders;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;
using Server.Main.Reactor.Models.Dto.Transactions;
using Server.Main.Reactor.Models.Enums;
using SqlKata.Execution;
using static Server.Main.Reactor.Builders.Tables.Generated.TransactionsTable;

namespace Server.Main.Reactor.Handlers.Domain;

public interface ITransactionDomainHandler
{
  IObservable<int> InsertTransaction(CreateTransactionDto dto, ulong subscriptionId);
  IObservable<TransactionsDto> UpdateTransaction(UpdateTransactionDto dto);
  IObservable<IEnumerable<TransactionsDto>> SelectTransactionsUsingId(ulong id);
  IObservable<TransactionsDto> SelectSettlementTransactionUsingId(ulong id);
  IObservable<IEnumerable<TransactionsDto>> SelectTransactionsUsingTransactionId(string? id);
  IObservable<TransactionsDto> SelectTransactionUsingTransactionId(string? id);
}

public class TransactionDomainHandler : ITransactionDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public TransactionDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<int> InsertTransaction(CreateTransactionDto dto, ulong subscriptionId)
  {
    return Observable.FromAsync(() => _queryFactory
      .Query(TableName)
      .InsertAsync(new TransactionRecordBuilder()
        .WithAmount(dto.Amount)
        .WithType(dto.Type)
        .WithCurrency("ZAR")
        .WithStatus(nameof(TransactionStatus.Pending))
        .WithTransactionDate(DateTime.UtcNow)
        .WithDescription(dto.Description ?? "Subscription Payment")
        .WithSubscriptionId(subscriptionId)
        .WithCreatedAt(DateTime.UtcNow)
        .Build())
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> UpdateTransaction(UpdateTransactionDto dto)
  {
    return SelectTransactionUsingTransactionId(dto.TransactionId)
      .Do(existingTransaction =>
      {
        if (existingTransaction.TransactionId == null)
        {
          throw new StandardException("Transaction not found.", StatusCodes.Status404NotFound);
        }
      })
      .Select(existingRecord =>
      {
        existingRecord.Status = dto.Status;
        existingRecord.UpdatedAt = DateTime.UtcNow;
        return existingRecord;
      })
      .SelectMany(record => _queryFactory.Query(TableName).UpdateAsync(record))
      .SelectMany(_ => SelectTransactionUsingTransactionId(dto.TransactionId))
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
