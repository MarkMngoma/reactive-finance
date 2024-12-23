using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Src.Main.Reactor.Handlers.Business;

namespace Src.Main.Reactor.Handlers.CrossCutting;

public class ThrowableHandler
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  public IObservable<IActionResult> Handle(Exception exception, int statusCode)
  {
    Logger.Error($"ThrowableHandler@HandleThrowable throwable result :: {exception}");
    return Observable.Return(new StatusCodeResult(statusCode));
  }
}
