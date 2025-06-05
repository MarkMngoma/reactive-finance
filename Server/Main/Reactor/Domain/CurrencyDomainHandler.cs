using System.Reactive.Concurrency;
using System.Reactive.Linq;
using SqlKata.Execution;
using Server.Main.Reactor.Builders;
using Server.Main.Reactor.Models.Request;
using static Server.Main.Reactor.Builders.Tables.Generated.CurrenciesTable;

namespace Server.Main.Reactor.Domain;

public class CurrencyDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public CurrencyDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<CurrencyRequest> SelectCurrencyUsingCode(string currencyCode)
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TableName)
        .Select(Id, CurrencyId, CurrencyCode, CurrencyName, CurrencySymbol, CurrencyFlag)
        .Where(CurrencyCode, currencyCode)
        .Limit(1)
        .FirstOrDefaultAsync<CurrencyRequest>()
      )
      .SubscribeOn(Scheduler.Default);
  }

  public IObservable<IEnumerable<CurrencyRequest>> SelectEnumerableCurrencies()
  {
    return Observable.FromAsync(() => _queryFactory.Query(TableName)
      .Select(Id, CurrencyId, CurrencyCode, CurrencyName, CurrencySymbol, CurrencyFlag)
      .GetAsync<CurrencyRequest>())
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<int> InsertCurrencyRecord(CurrencyRequest request)
  {
    return Observable.FromAsync(() => _queryFactory
      .Query(TableName)
      .InsertAsync(new CurrencyRecordBuilder()
        .WithCurrencyCode(request.CurrencyCode)
        .WithCurrencySymbol(request.CurrencySymbol)
        .WithCurrencyFlag(request.CurrencyFlag)
        .WithCurrencyName(request.CurrencyName)
        .WithCurrencyId(request.CurrencyId)
        .Build())
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<int> InsertBatchCurrencyRecords(BatchCurrencyRequest request)
  {
    var batchRecords = new InsertBatchCurrencyRecordBuilder()
      .AddCurrencyEntry(request)
      .Build();
    return Observable.FromAsync(() => _queryFactory
      .Query(TableName)
      .InsertAsync(batchRecords.Columns, batchRecords.Records))
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<int> DeleteCurrencyRecords()
  {
    return Observable.FromAsync(() => _queryFactory
      .Query(TableName)
      .DeleteAsync())
      .SubscribeOn(TaskPoolScheduler.Default);
  }
}
