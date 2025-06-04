using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class WriteCurrenciesHandler : IHandler<CurrencyRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly CurrencyDao _currencyDao;
  private readonly QueryCurrenciesHandler _handler;

  public WriteCurrenciesHandler(CurrencyDao currencyDao, QueryCurrenciesHandler handler)
  {
    _currencyDao = currencyDao;
    _handler = handler;
  }

  public IObservable<JsonResult> Handle(CurrencyRequest request)
  {
    return _currencyDao.InsertCurrencyRecord(request)
      .Select(_ => request)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .SelectMany(_ => _currencyDao.SelectCurrencyUsingCode(request.CurrencyCode))
      .Do(dataResult => Logger.Debug($"WriteCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .SelectMany(ContentResultUtil.Render)
      .Finally(() => Logger.Info($"SubscribedOn: {Thread.CurrentThread.Name}"))
      .SubscribeOn(new EventLoopScheduler());
  }
}
