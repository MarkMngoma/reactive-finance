using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class DeleteSubscriptionHandler : IHandler<DeleteSubscriptionRequest>
{
  public IObservable<JsonResult> Handle(DeleteSubscriptionRequest request)
  {
    throw new NotImplementedException();
  }
}
