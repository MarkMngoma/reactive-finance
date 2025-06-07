using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class DeleteSubscriptionHandler : Handler<DeleteSubscriptionRequest>
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(DeleteSubscriptionHandler));

  public override IObservable<JsonResult> Handle(DeleteSubscriptionRequest request)
  {
    Logger.Info($"Handling DeleteSubscriptionRequest for SubscriptionId: {request.SubscriptionId}");
    throw new NotImplementedException();
  }
}
