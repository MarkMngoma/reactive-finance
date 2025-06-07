using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Transactions;
using Server.Main.Reactor.Models.Response;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class CreateAdhocTransactionHandler : Handler<CreateTransactionRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateAdhocTransactionHandler));

  public override IObservable<JsonResult> Handle(CreateTransactionRequest request)
  {
    Logger.Info("CreateAdhocTransactionHandler@Handle called");
    return HandleComputeEvent(request)
      .SelectMany(HandleAuthorisation)
      .Select(_ => ContentResultUtil.Render(new BasicResponse(true)));
  }

  public IObservable<JsonResult> HandleAuthorisation(CreateTransactionRequest request)
  {
    Logger.Info($"CreateAdhocTransactionHandler@HandleAuthorisation called");
    return Handle(request);
  }
}
