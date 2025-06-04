using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Models.Request.Transactions;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class QueryRefundEligibilityHandler : IHandler<CreateTransactionRequest>
{
  public IObservable<JsonResult> Handle(CreateTransactionRequest request)
  {
    throw new NotImplementedException();
  }
}
