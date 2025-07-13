using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.Dto.Currencies;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class WriteBatchCurrenciesHandler : Handler<BatchCurrencyDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly ICurrencyDomainHandler _currencyDomainHandler;

  public WriteBatchCurrenciesHandler(ICurrencyDomainHandler currencyDomainHandler)
  {
    _currencyDomainHandler = currencyDomainHandler;
  }

  public override IObservable<JsonResult> Handle(BatchCurrencyDto dto)
  {
    Logger.Info($"WriteBatchCurrenciesHandler@Handle initiated with request size #{dto.BatchCurrencies.Count}");
    return HandleComputeEvent(dto)
      .SelectMany(_ => _currencyDomainHandler.DeleteCurrencyRecords())
      .SelectMany(_ => _currencyDomainHandler.InsertBatchCurrencyRecords(dto))
      .Do(dataResult => Logger.Info($"WriteBatchCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .SelectMany(_ => _currencyDomainHandler.SelectEnumerableCurrencies())
      .Do(dataResult => Logger.Info($"WriteBatchCurrenciesHandler@Handle domain result :: {dataResult.ToList().Count}"))
      .Select(ContentResultUtil.Render);
  }

}
