using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Dto.Transactions;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class QueryRefundEligibilityHandler : Handler<UpdateTransactionDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateAdhocTransactionHandler));

  public override IObservable<JsonResult> Handle(UpdateTransactionDto dto)
  {
    Logger.Info($"QueryRefundEligibilityHandler@Handle initiated for TransactionId: {dto.TransactionId}");
    throw new NotImplementedException();
  }
}
