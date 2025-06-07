using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class CreateSubscriptionHandler : Handler<CreateSubscriptionRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateSubscriptionHandler));

  public override IObservable<JsonResult> Handle(CreateSubscriptionRequest request)
  {
    Logger.Info($"CreateSubscriptionHandler@Handle initiated...");
    throw new NotImplementedException();
  }
}
