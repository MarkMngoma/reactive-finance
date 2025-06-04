using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class WriteBatchCurrenciesHandler : IHandler<BatchCurrencyRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly CurrencyDao _currencyDao;
  private readonly QueryCurrenciesHandler _handler;

  public WriteBatchCurrenciesHandler(CurrencyDao currencyDao, QueryCurrenciesHandler handler)
  {
    _currencyDao = currencyDao;
    _handler = handler;
  }

  public IObservable<JsonResult> Handle(BatchCurrencyRequest request)
  {
    Logger.Debug($"WriteBatchCurrenciesHandler@Handle initiated with request size #{request.BatchCurrencies.Count}");
    return _currencyDao.InsertBatchCurrencyRecords(request)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .Do(dataResult => Logger.Debug($"WriteBatchCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .SelectMany(_ => _handler.QueryCollectiveCurrencies())
      .Finally(() => Logger.Info($"SubscribedOn: {Thread.CurrentThread.Name}"))
      .SubscribeOn(new EventLoopScheduler());
  }
}
