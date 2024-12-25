using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using Src.Main.Reactor.Handlers.CrossCutting;
using Src.Main.Reactor.Models.Dto;

namespace Src.Main.Reactor.Handlers.Business;

public class WriteBatchCurrenciesHandler : ICommandHandler<BatchCurrencyDto, JsonResult>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private const string TableName = "CURRENCIES";

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
      .SelectMany(batchRecords => Observable.FromAsync(() => _commandQueryFactory.Query(TableName).InsertAsync([
        "CURRENCY_ID",
        "CURRENCY_CODE",
        "CURRENCY_SYMBOL",
        "CURRENCY_FLAG",
        "CURRENCY_NAME",
        "ARCHIVED",
        "CREATED_AT",
        "CREATED_BY"
      ], batchRecords)))
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Debug($"WriteBatchCurrenciesHandler@Handle domain result :: {dataResult}"))
      .SelectMany(_ => _handler.QueryCollectiveCurrencies());
  }

  private List<object[]> ConstructBatchCurrenciesRecords(BatchCurrencyDto request)
  {
    var records = new List<object[]>();
    request.BatchCurrencies.ForEach(requestItem =>
    {
      records.Add([
        requestItem.CurrencyId,
        requestItem.CurrencyCode,
        requestItem.CurrencySymbol,
        requestItem.CurrencyFlag,
        requestItem.CurrencyName,
        0,
        DateTimeOffset.Now,
        1
      ]);
    });

    return records;
  }
}
