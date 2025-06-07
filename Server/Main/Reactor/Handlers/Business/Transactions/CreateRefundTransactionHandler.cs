using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Transactions;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class CreateRefundTransactionHandler : Handler<UpdateTransactionRequest>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateRefundTransactionHandler));

  public override IObservable<JsonResult> Handle(UpdateTransactionRequest request)
  {
    Logger.Info($"CreateRefundTransactionHandler@Handle initiated for TransactionId: {request.TransactionId}");
    throw new NotImplementedException();
  }
}
