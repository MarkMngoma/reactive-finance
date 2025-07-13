using Microsoft.AspNetCore.Mvc;

namespace Server.Main.Reactor.Handlers.CrossCutting;

public interface IHandler<TR>
{
  IObservable<JsonResult> Handle(TR request);
}
