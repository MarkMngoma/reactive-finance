using System;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;

namespace Server.Main.Reactor.Handlers.Business.Transactions;

public class QueryTransactionHistoryHandler : Handler<string>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryTransactionHistoryHandler));

  public override IObservable<JsonResult> Handle(string request)
  {
    Logger.Info($"QueryTransactionHistoryHandler@Handle initiated for TransactionId: {request}");
    throw new NotImplementedException();
  }
}
