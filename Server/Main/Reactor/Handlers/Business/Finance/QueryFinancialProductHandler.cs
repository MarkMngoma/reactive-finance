using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Domain;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.CrossCutting.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class QueryFinancialProductHandler : Handler<string>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryFinancialProductHandler));

  private readonly FinancialProductDomainHandler _financialProductDomainHandler;

  public QueryFinancialProductHandler(FinancialProductDomainHandler financialProductDomainHandler)
  {
    _financialProductDomainHandler = financialProductDomainHandler;
  }

  public override IObservable<JsonResult> Handle(string id)
  {
    return ExecCompute(id)
      .SelectMany(_financialProductDomainHandler.SelectFinancialProductUsingId)
      .Do(dataResult => Logger.Debug($"QueryFinancialProductHandler@Handle domain result :: {dataResult}"))
      .Select(ContentResultUtil.Render);
  }

  public IObservable<JsonResult> Handle()
  {
    return ExecCompute(_financialProductDomainHandler.SelectFinancialProducts())
      .Do(dataResult => Logger.Debug($"QueryFinancialProductHandler@Handle domain result :: {dataResult.ToList().Count}"))
      .Select(ContentResultUtil.Render);
  }
}
