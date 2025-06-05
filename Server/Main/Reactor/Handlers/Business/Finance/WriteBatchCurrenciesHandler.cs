using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class WriteBatchCurrenciesHandler : Handler<BatchCurrencyRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly CurrencyDomainHandler _currencyDomainHandler;
  private readonly QueryCurrenciesHandler _queryCurrenciesHandler;

  public WriteBatchCurrenciesHandler(CurrencyDomainHandler currencyDomainHandler, QueryCurrenciesHandler queryCurrenciesHandler)
  {
    _currencyDomainHandler = currencyDomainHandler;
    _queryCurrenciesHandler = queryCurrenciesHandler;
  }

  public override IObservable<JsonResult> Handle(BatchCurrencyRequest request)
  {
    Logger.Debug($"WriteBatchCurrenciesHandler@Handle initiated with request size #{request.BatchCurrencies.Count}");
    return ExecCompute(request)
      .SelectMany(_currencyDomainHandler.InsertBatchCurrencyRecords)
      .SelectMany(_ => _currencyDomainHandler.DeleteCurrencyRecords())
      .Do(dataResult => Logger.Debug($"WriteBatchCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .SelectMany(_ => _queryCurrenciesHandler.HandleCurrencyListQuery());
  }
}
