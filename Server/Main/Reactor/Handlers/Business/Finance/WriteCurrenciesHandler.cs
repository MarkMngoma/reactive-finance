using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.CrossCutting.Utils;
using Server.Main.Reactor.Models.Request;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class WriteCurrenciesHandler : Handler<CurrencyRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly CurrencyDomainHandler _currencyDomainHandler;
  private readonly QueryCurrenciesHandler _queryCurrenciesHandler;

  public WriteCurrenciesHandler(CurrencyDomainHandler _currencyDomainHandler, QueryCurrenciesHandler _queryCurrenciesHandler)
  {
    this._currencyDomainHandler = _currencyDomainHandler;
    this._queryCurrenciesHandler = _queryCurrenciesHandler;
  }

  public override IObservable<JsonResult> Handle(CurrencyRequest request)
  {
    Logger.Debug($"WriteCurrenciesHandler@Handle initiated with request :: {JsonSerializer.Serialize(request)}");
    return ExecCompute(request)
      .SelectMany(_currencyDomainHandler.InsertCurrencyRecord)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .SelectMany(_ => _currencyDomainHandler.SelectCurrencyUsingCode(request.CurrencyCode))
      .Do(dataResult => Logger.Debug($"WriteCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .Select(ContentResultUtil.Render);
  }
}
