using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Clients;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Dto.Queries;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class QueryExchangeRateHandler : Handler<QueryCurrencyDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryExchangeRateHandler));

  private readonly FxHttpClient _fxHttpClient;

  public QueryExchangeRateHandler(FxHttpClient fxHttpClient)
  {
    _fxHttpClient = fxHttpClient;
  }

  public override IObservable<JsonResult> Handle(QueryCurrencyDto dto)
  {
    Logger.Info("QueryCurrenciesHandler@Handle initiated...");
    return _fxHttpClient.QueryExternalPartyExchangeRates(dto?.QuoteCurrencyCode)
      .Do(dto => Logger.Info($"QueryCurrenciesHandler@Handle preparing response :: {dto}"))
      .Select(ContentResultUtil.Render);
  }
}
