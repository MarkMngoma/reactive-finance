using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace server.za.co.finance.handlers.crosscutting;

public class ThrowableHandler
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  public IObservable<IActionResult> Handle(Exception exception, int statusCode)
  {
    Logger.Info($"ThrowableHandler@HandleThrowable throwable result :: {exception}");
    return Observable.Return(new StatusCodeResult(statusCode));
  }
}
