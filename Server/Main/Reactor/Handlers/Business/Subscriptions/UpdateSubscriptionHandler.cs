using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Dto.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class UpdateSubscriptionHandler : Handler<UpdateSubscriptionDto>
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(UpdateSubscriptionHandler));

  public override IObservable<JsonResult> Handle(UpdateSubscriptionDto dto)
  {
    Logger.Info($"Handling UpdateSubscriptionRequest for SubscriptionId: {dto.SubscriptionId}");
    throw new NotImplementedException();
  }
}
