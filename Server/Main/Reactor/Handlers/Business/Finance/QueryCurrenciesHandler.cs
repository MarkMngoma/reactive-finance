using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.Dto.Queries;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class QueryCurrenciesHandler : Handler<QueryCurrencyDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly ICurrencyDomainHandler _currencyDomainHandler;

  public QueryCurrenciesHandler(ICurrencyDomainHandler currencyDomainHandler)
  {
    _currencyDomainHandler = currencyDomainHandler;
  }

  public override IObservable<JsonResult> Handle(QueryCurrencyDto dto)
  {
    Logger.Info($"QueryCurrenciesHandler@Handle initiated for :: {dto.CurrencyCode}");
    return HandleComputeEvent(dto)
      .SelectMany(r => _currencyDomainHandler.SelectCurrencyUsingCode(r?.CurrencyCode))
      .Do(dataResult => Logger.Info($"QueryCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .Select(ContentResultUtil.Render);
  }

  public IObservable<JsonResult> HandleCurrencyListQuery()
  {
    Logger.Info("QueryCurrenciesHandler@HandleCurrencyListQuery initiated...");
    return HandleComputeEvent(_currencyDomainHandler.SelectEnumerableCurrencies())
      .Do(dataResult => Logger.Info($"QueryCurrenciesHandler@HandleCurrencyListQuery domain result :: {dataResult.ToList().Count}"))
      .Select(ContentResultUtil.Render);
  }

}
