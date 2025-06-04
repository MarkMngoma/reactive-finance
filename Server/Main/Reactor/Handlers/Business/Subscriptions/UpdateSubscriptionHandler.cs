using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class UpdateSubscriptionHandler : IHandler<UpdateSubscriptionRequest>
{

  public IObservable<JsonResult> Handle(UpdateSubscriptionRequest request)
  {
    throw new NotImplementedException();
  }
}
