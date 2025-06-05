using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Server.Main.Reactor.Builders;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;
using Server.Main.Reactor.Models.Request.Transactions;
using SqlKata.Execution;

using static Server.Main.Reactor.Builders.Tables.Generated.TransactionsTable;
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
      .Query(TableName)
      .InsertAsync(record))
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> UpdateTransaction(UpdateTransactionRequest request)
  {
    return SelectTransactionById(request.TransactionId)
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
      .SelectMany(_ => SelectTransactionById(request.TransactionId))
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<TransactionsDto> SelectTransactionById(string? searchId)
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TableName)
          .Select(TransactionId, SubscriptionId, TransactionsTable.Type, Amount, Currency, Status, AuthorizationId, TransactionDate, Description)
          .Where(TransactionId, searchId)
          .OrWhere(SubscriptionId, searchId)
          .OrWhere(AuthorizationId, searchId)
          .OrWhere(Id, searchId)
          .FirstOrDefaultAsync<TransactionsDto>()
      )
      .DefaultIfEmpty(new TransactionsDto())
      .SubscribeOn(TaskPoolScheduler.Default);
  }
}
