using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.DTO;
using Server.Main.Reactor.Models.Dto.Transactions;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class CreateAdhocTransactionHandler : Handler<CreateTransactionDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateAdhocTransactionHandler));

  public override IObservable<JsonResult> Handle(CreateTransactionDto dto)
  {
    Logger.Info("CreateAdhocTransactionHandler@Handle called");
    return HandleComputeEvent(dto)
      .SelectMany(HandleAuthorisation)
      .Select(_ => ContentResultUtil.Render(new BasicResponseDto(true)));
  }

  private IObservable<JsonResult> HandleAuthorisation(CreateTransactionDto dto)
  {
    Logger.Info($"CreateAdhocTransactionHandler@HandleAuthorisation called");
    return Handle(dto);
  }
}
