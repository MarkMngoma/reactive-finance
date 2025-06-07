using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.Request;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class WriteCurrenciesHandler : Handler<CurrencyRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly CurrencyDomainHandler _currencyDomainHandler;

  public WriteCurrenciesHandler(CurrencyDomainHandler _currencyDomainHandler)
  {
    this._currencyDomainHandler = _currencyDomainHandler;
  }

  public override IObservable<JsonResult> Handle(CurrencyRequest request)
  {
    Logger.Debug($"WriteCurrenciesHandler@Handle initiated...");
    return HandleComputeEvent(request)
      .SelectMany(_currencyDomainHandler.InsertCurrencyRecord)
      .SelectMany(_ => _currencyDomainHandler.SelectCurrencyUsingCode(request.CurrencyCode))
      .Do(dataResult => Logger.Debug($"WriteCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .Select(ContentResultUtil.Render);
  }
}
