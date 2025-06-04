using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class CreateSubscriptionHandler : IHandler<CreateSubscriptionRequest>
{

  public IObservable<JsonResult> Handle(CreateSubscriptionRequest request)
  {
    throw new NotImplementedException();
  }
}
