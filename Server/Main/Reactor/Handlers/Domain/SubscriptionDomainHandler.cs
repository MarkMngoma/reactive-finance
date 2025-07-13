using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Server.Main.Reactor.Builders;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using SqlKata.Execution;
using static Server.Main.Reactor.Builders.Tables.Generated.SubscriptionsTable;

namespace Server.Main.Reactor.Handlers.Domain;

public interface ISubscriptionDomainHandler
{
  IObservable<SubscriptionsDto> SelectSubscriptionUsingId(ulong id);
  IObservable<IEnumerable<SubscriptionsDto>> SelectSubscriptions();
  IObservable<int> InsertCurrencyRecord(SubscriptionsDto request);
}

public class SubscriptionDomainHandler : ISubscriptionDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public SubscriptionDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<SubscriptionsDto> SelectSubscriptionUsingId(ulong id)
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TableName)
        .Select(Id, BusinessId, FinancialProductId, Status, StartDate, EndDate, CreatedAt)
        .Where(Id, id)
        .Limit(1)
        .FirstOrDefaultAsync<SubscriptionsDto>()
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<IEnumerable<SubscriptionsDto>> SelectSubscriptions()
  {
    return Observable.FromAsync(() => _queryFactory.Query(TableName)
      .Select(Id, BusinessId, FinancialProductId, Status, StartDate, EndDate, CreatedAt)
      .GetAsync<SubscriptionsDto>()
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<int> InsertCurrencyRecord(SubscriptionsDto request)
  {
    return Observable.FromAsync(() => _queryFactory
      .Query(TableName)
      .InsertAsync(new SubscriptionRecordBuilder()
        .WithBusinessId(request.BusinessId)
        .WithFinancialProductId(request.FinancialProductId)
        .WithStatus(request.Status)
        .WithStartDate(request.StartDate)
        .WithEndDate(request.EndDate)
        .WithCreatedAt(request.CreatedAt)
        .Build())
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

}
