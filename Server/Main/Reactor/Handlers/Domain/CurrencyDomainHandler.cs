using System.Reactive.Concurrency;
using System.Reactive.Linq;
using SqlKata.Execution;
using Server.Main.Reactor.Builders;
using Server.Main.Reactor.Models.Dto.Currencies;
using static Server.Main.Reactor.Builders.Tables.Generated.CurrenciesTable;

namespace Server.Main.Reactor.Handlers.Domain;

public interface ICurrencyDomainHandler
{
  IObservable<CurrencyDto> SelectCurrencyUsingCode(string? currencyCode);
  IObservable<bool> SelectCurrencyExistsUsingCode(string? currencyCode);
  IObservable<IEnumerable<CurrencyDto>> SelectEnumerableCurrencies();
  IObservable<int> InsertCurrencyRecord(CurrencyDto dto);
  IObservable<int> InsertBatchCurrencyRecords(BatchCurrencyDto dto);
  IObservable<int> DeleteCurrencyRecords();
}

public class CurrencyDomainHandler : ICurrencyDomainHandler
{
  private readonly QueryFactory _queryFactory;

  public CurrencyDomainHandler(QueryFactory queryFactory)
  {
    _queryFactory = queryFactory;
  }

  public IObservable<CurrencyDto> SelectCurrencyUsingCode(string? currencyCode)
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TableName)
        .Select(Id, CurrencyId, CurrencyCode, CurrencyName, CurrencySymbol, CurrencyFlag)
        .Where(CurrencyCode, currencyCode)
        .Limit(1)
        .FirstOrDefaultAsync<CurrencyDto>()
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<bool> SelectCurrencyExistsUsingCode(string? currencyCode)
  {
    return Observable.FromAsync(() =>
        _queryFactory.Query(TableName)
          .Where(CurrencyCode, currencyCode)
          .ExistsAsync()
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<IEnumerable<CurrencyDto>> SelectEnumerableCurrencies()
  {
    return Observable.FromAsync(() => _queryFactory.Query(TableName)
      .Select(Id, CurrencyId, CurrencyCode, CurrencyName, CurrencySymbol, CurrencyFlag)
      .GetAsync<CurrencyDto>())
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<int> InsertCurrencyRecord(CurrencyDto dto)
  {
    return Observable.FromAsync(() => _queryFactory
      .Query(TableName)
      .InsertAsync(new CurrencyRecordBuilder()
        .WithCurrencyCode(dto.CurrencyCode)
        .WithCurrencySymbol(dto.CurrencySymbol)
        .WithCurrencyFlag(dto.CurrencyFlag)
        .WithCurrencyName(dto.CurrencyName)
        .WithCurrencyId(dto.CurrencyId)
        .Build())
      )
      .SubscribeOn(TaskPoolScheduler.Default);
  }

  public IObservable<int> InsertBatchCurrencyRecords(BatchCurrencyDto dto)
  {
    var batchRecords = new InsertBatchCurrencyRecordBuilder()
      .AddCurrencyEntry(dto)
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
