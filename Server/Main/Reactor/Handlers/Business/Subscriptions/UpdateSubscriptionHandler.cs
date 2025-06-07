using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class UpdateSubscriptionHandler : Handler<UpdateSubscriptionRequest>
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(UpdateSubscriptionHandler));

  public override IObservable<JsonResult> Handle(UpdateSubscriptionRequest request)
  {
    Logger.Info($"Handling UpdateSubscriptionRequest for SubscriptionId: {request.SubscriptionId}");
    throw new NotImplementedException();
  }
}
