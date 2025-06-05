using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Server.Main.Reactor.Handlers.CrossCutting;

public abstract class Handler<R>
{
  public abstract IObservable<JsonResult> Handle(R request);

  public IObservable<R> ExecCompute(R request)
  {
    return Observable.Using(() => new EventLoopScheduler(), eventLoopScheduler =>
        Observable.Return(request)
          .Retry(3)
          .Timeout(TimeSpan.FromMilliseconds(2000))
          .ObserveOn(eventLoopScheduler)
          .Finally(eventLoopScheduler.Dispose)
      );
  }

  public IObservable<T> ExecCompute<T>(IObservable<T> source)
  {
      return Observable.Using(() => new EventLoopScheduler(), eventLoopScheduler => source
        .Retry(3)
        .Timeout(TimeSpan.FromMilliseconds(2000))
        .ObserveOn(eventLoopScheduler)
        .Finally(eventLoopScheduler.Dispose)
      );
  }
}
