using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using SqlKata.Execution;
using static Server.Main.Reactor.Builders.Tables.Generated.FinancialProductTable;

namespace Server.Main.Reactor.Handlers.Domain;

public interface IFinancialProductDomainHandler
{
  IObservable<IEnumerable<FinancialProductDto>> SelectFinancialProducts();
  IObservable<FinancialProductDto> SelectFinancialProductUsingId(string? id);
}

public class FinancialProductDomainHandler : IFinancialProductDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public FinancialProductDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<IEnumerable<FinancialProductDto>> SelectFinancialProducts()
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TableName)
          .Select(Id, Name, Description, BillingCycle, Price)
          .GetAsync<FinancialProductDto>()
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<FinancialProductDto> SelectFinancialProductUsingId(string? id)
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TableName)
          .Select(Id, Name, Description, BillingCycle, Price)
          .Where(Id, id)
          .FirstOrDefaultAsync<FinancialProductDto>()
      )
      .DefaultIfEmpty(new FinancialProductDto())
      .SubscribeOn(TaskPoolScheduler.Default);
  }

}
