using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Dto.Subscriptions;

namespace Server.Main.Reactor.Handlers.Business.Subscriptions;

public class CreateSubscriptionHandler : Handler<CreateSubscriptionDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateSubscriptionHandler));

  public override IObservable<JsonResult> Handle(CreateSubscriptionDto dto)
  {
    Logger.Info($"CreateSubscriptionHandler@Handle initiated...");
    throw new NotImplementedException();
  }
}
