using Microsoft.AspNetCore.Mvc;

namespace Server.Main.Reactor.Handlers.CrossCutting;

public interface IHandler<R>
{
  public IObservable<JsonResult> Handle(R request);
}
