using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Builders.Tables.Generated;
using Server.Main.Reactor.Builders.Tables.Generated.Models;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.Dto.Queries;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;


public class QueryFinancialProductHandler : Handler<QueryFinancialProductDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryFinancialProductHandler));

  private readonly IFinancialProductDomainHandler _financialProductDomainHandler;
  private readonly ICachingHandler _cachingHandler;

  public QueryFinancialProductHandler(IFinancialProductDomainHandler financialProductDomainHandler, ICachingHandler cachingHandler)
  {
    _financialProductDomainHandler = financialProductDomainHandler;
    _cachingHandler = cachingHandler;
  }

  public override IObservable<JsonResult> Handle(QueryFinancialProductDto dto)
  {
    return HandleComputeEvent(dto)
      .SelectMany(r => _cachingHandler.HandleGet<FinancialProductDto>(FinancialProductTable.TableName, r?.Id))
      .SelectMany(data => data?.Id != null ? Observable.Return(data) : HandleReadThenWriteIntoCache(dto?.Id))
      .Do(
        onNext: result => Logger.Info($"QueryFinancialProductHandler@Handle domain result :: {result}"),
        onError: ex => Logger.Info($"QueryFinancialProductHandler@Handle throwable result :: {ex.StackTrace}")
      )
      .Select(ContentResultUtil.Render);
  }

  public IObservable<JsonResult> Handle()
  {
    return HandleComputeEvent(_cachingHandler.HandleGet<IEnumerable<FinancialProductDto>>(FinancialProductTable.TableName, "fp-collection"))
      .SelectMany(data =>  data != null ? Observable.Return(data) : HandleReadThenWriteIntoCache())
      .Do(dataResult => Logger.Info($"QueryFinancialProductHandler@Handle domain result :: {dataResult.ToList().Count}"))
      .Select(ContentResultUtil.Render);
  }

  private IObservable<FinancialProductDto> HandleReadThenWriteIntoCache(string id)
  {
    Logger.Debug($"QueryFinancialProductHandler@HandleReadThenWriteIntoCache {id}, querying database...");
    return _financialProductDomainHandler.SelectFinancialProductUsingId(id)
      .SelectMany(financialProductDto => _cachingHandler.HandleWrite(FinancialProductTable.TableName, id, financialProductDto));
  }

  private IObservable<IEnumerable<FinancialProductDto>> HandleReadThenWriteIntoCache()
  {
    Logger.Debug($"QueryFinancialProductHandler@HandleReadThenWriteIntoCache querying database...");
    return _financialProductDomainHandler.SelectFinancialProducts()
      .SelectMany(financialProductDto => _cachingHandler.HandleWrite(FinancialProductTable.TableName, "fp-collection", financialProductDto));
  }

}
