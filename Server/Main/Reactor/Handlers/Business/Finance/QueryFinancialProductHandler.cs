using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class QueryFinancialProductHandler : Handler<string>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryFinancialProductHandler));

  private readonly FinancialProductDomainHandler _financialProductDomainHandler;
  private readonly CachingHandler _cachingHandler;

  public QueryFinancialProductHandler(FinancialProductDomainHandler financialProductDomainHandler, CachingHandler cachingHandler)
  {
    _financialProductDomainHandler = financialProductDomainHandler;
    _cachingHandler = cachingHandler;
  }

  public override IObservable<JsonResult> Handle(string id)
  {
    return HandleComputeEvent(id)
      .SelectMany(cacheId => _cachingHandler.HandleGet<FinancialProductDto>(FinancialProductTable.TableName, cacheId))
      .SelectMany(data => (data != null) ? Observable.Return(data) : HandleReadThenWriteIntoCache(id))
      .Do(dataResult => Logger.Debug($"QueryFinancialProductHandler@Handle domain result :: {dataResult}"))
      .Select(ContentResultUtil.Render);
  }

  public IObservable<JsonResult> Handle()
  {
    return HandleComputeEvent(HandleReadThenWriteIntoCache())
      .Do(dataResult => Logger.Debug($"QueryFinancialProductHandler@Handle domain result :: {dataResult.ToList().Count}"))
      .Select(ContentResultUtil.Render);
  }

  private IObservable<FinancialProductDto> HandleReadThenWriteIntoCache(string id)
  {
    Logger.Debug($"QueryFinancialProductHandler@HandleReadThenWriteIntoCache {id}, querying database...");
    return _financialProductDomainHandler.SelectFinancialProductUsingId(id)
      .SelectMany(dbData => _cachingHandler.HandleWrite(FinancialProductTable.TableName, id, dbData));
  }

  private IObservable<IEnumerable<FinancialProductDto>> HandleReadThenWriteIntoCache()
  {
    Logger.Debug($"QueryFinancialProductHandler@HandleReadThenWriteIntoCache querying database...");
    return _financialProductDomainHandler.SelectFinancialProducts()
      .SelectMany(dbData => _cachingHandler.HandleWrite(FinancialProductTable.TableName, "fp-collection", dbData));
  }

}
