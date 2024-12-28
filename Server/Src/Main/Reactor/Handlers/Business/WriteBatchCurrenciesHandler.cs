using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using Src.Main.Reactor.Builders;
using Src.Main.Reactor.Builders.Tables.Generated;
using Src.Main.Reactor.Handlers.CrossCutting;
using Src.Main.Reactor.Models.Dto;

namespace Src.Main.Reactor.Handlers.Business;

public class WriteBatchCurrenciesHandler : ICommandHandler<BatchCurrencyDto, JsonResult>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly QueryFactory _commandQueryFactory;
  private readonly QueryCurrenciesHandler _handler;

  public WriteBatchCurrenciesHandler(QueryFactory commandQueryFactory, QueryCurrenciesHandler handler)
  {
    _commandQueryFactory = commandQueryFactory;
    _handler = handler;
  }

  public IObservable<JsonResult> Handle(BatchCurrencyDto request)
  {
    Logger.Debug($"WriteBatchCurrenciesHandler@Handle initiated with request size #{request.BatchCurrencies.Count}");
    return Observable.Return(request)
      .Select(ConstructBatchCurrenciesRecords)
      .SelectMany(batchRecords => Observable.FromAsync(() => _commandQueryFactory.Query(CurrenciesTable.TableName).InsertAsync(batchRecords.Columns, batchRecords.Records)))
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Debug($"WriteBatchCurrenciesHandler@Handle domain result :: {dataResult}"))
      .SelectMany(_ => _handler.QueryCollectiveCurrencies());
  }

  private (IReadOnlyList<string> Columns, List<object[]> Records) ConstructBatchCurrenciesRecords(BatchCurrencyDto request)
  {
    return new InsertBatchCurrencyRecordBuilder()
      .AddCurrencyEntry(request)
      .Build();
  }
}
