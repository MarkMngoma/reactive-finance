using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Transactions;
using Server.Main.Reactor.Models.Response;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class CreateRefundTransactionHandler : IHandler<CreateTransactionRequest>
{
  public IObservable<JsonResult> Handle(CreateTransactionRequest request)
  {
    throw new NotImplementedException();
  }
}
