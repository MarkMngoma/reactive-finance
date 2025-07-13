using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Server.Main.Reactor.Handlers.CrossCutting;

public abstract class Handler<TR> : IHandler<TR>
{
  protected IObservable<TR> HandleComputeEvent(TR request)
  {
    return Observable.Using(() => new EventLoopScheduler(), eventLoopScheduler =>
        Observable.Return(request)
          .Retry(3)
          .Timeout(TimeSpan.FromMilliseconds(2000))
          .ObserveOn(eventLoopScheduler)
          .Finally(eventLoopScheduler.Dispose)
      );
  }

  protected IObservable<T> HandleComputeEvent<T>(IObservable<T> source)
  {
      return Observable.Using(() => new EventLoopScheduler(), eventLoopScheduler => source
        .Retry(3)
        .Timeout(TimeSpan.FromMilliseconds(2000))
        .ObserveOn(eventLoopScheduler)
        .Finally(eventLoopScheduler.Dispose)
      );
  }

  public abstract IObservable<JsonResult> Handle(TR request);
}
