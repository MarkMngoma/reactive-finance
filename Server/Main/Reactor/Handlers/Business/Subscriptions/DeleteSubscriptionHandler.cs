using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Dto.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class DeleteSubscriptionHandler : Handler<DeleteSubscriptionDto>
{

  private static readonly ILog Logger = LogManager.GetLogger(typeof(DeleteSubscriptionHandler));

  public override IObservable<JsonResult> Handle(DeleteSubscriptionDto dto)
  {
    Logger.Info($"Handling DeleteSubscriptionRequest for SubscriptionId: {dto.SubscriptionId}");
    throw new NotImplementedException();
  }
}
