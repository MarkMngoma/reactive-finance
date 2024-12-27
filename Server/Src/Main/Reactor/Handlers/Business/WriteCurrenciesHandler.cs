using System.Reactive.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using Src.Main.Infrastructure.Builders;
using Src.Main.Reactor.Handlers.CrossCutting;
using Src.Main.Reactor.Models.Dto;

namespace Src.Main.Reactor.Handlers.Business;

public class WriteCurrenciesHandler : ICommandHandler<CurrencyDto, JsonResult>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private const string TableName = "CURRENCIES";

  private readonly QueryFactory _commandQueryFactory;
  private readonly QueryCurrenciesHandler _handler;
  private readonly ContentResultHandler _contentResultHandler;

  public WriteCurrenciesHandler(QueryFactory commandQueryFactory, QueryCurrenciesHandler handler,
    ContentResultHandler contentResultHandler)
  {
    _commandQueryFactory = commandQueryFactory;
    _handler = handler;
    _contentResultHandler = contentResultHandler;
  }

  public IObservable<JsonResult> Handle(CurrencyDto request)
  {
    return Observable.FromAsync(() => _commandQueryFactory.Query(TableName).InsertAsync( new InsertCurrencyRecordBuilder(request).Build()))
      .Do(dataResult => Logger.Debug($"WriteCurrenciesHandler@Handle domain result :: {dataResult}"))
      .Select(_ => request)
      .Timeout(TimeSpan.FromMilliseconds(2000))
      .SelectMany(_ => _handler.FetchCurrencyUsingCode(request.CurrencyCode))
      .SelectMany(httpResult => _contentResultHandler.RenderContentResult(httpResult));
  }
}
