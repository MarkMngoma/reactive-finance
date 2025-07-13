using System.Reactive.Linq;
using System.Text.Json;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Server.Main.Reactor.Handlers.CrossCutting;
using Server.Main.Reactor.Handlers.CrossCutting.Exceptions;
using Server.Main.Reactor.Handlers.Domain;
using Server.Main.Reactor.Models.Dto.Currencies;
using Server.Main.Reactor.Utils;

namespace Server.Main.Reactor.Handlers.Business.Finance;

public class WriteCurrenciesHandler : Handler<CurrencyDto>
{
  private static readonly ILog Logger = LogManager.GetLogger(typeof(QueryCurrenciesHandler));

  private readonly ICurrencyDomainHandler _currencyDomainHandler;

  public WriteCurrenciesHandler(ICurrencyDomainHandler currencyDomainHandler)
  {
    _currencyDomainHandler = currencyDomainHandler;
  }

  public override IObservable<JsonResult> Handle(CurrencyDto dto)
  {
    Logger.Debug($"WriteCurrenciesHandler@Handle initiated...");
    return HandleComputeEvent(dto)
      .SelectMany(HandleDuplicateEntryCheck)
      .SelectMany(_currencyDomainHandler.InsertCurrencyRecord)
      .SelectMany(_ => _currencyDomainHandler.SelectCurrencyUsingCode(dto.CurrencyCode))
      .Do(dataResult => Logger.Debug($"WriteCurrenciesHandler@Handle domain result :: {JsonSerializer.Serialize(dataResult)}"))
      .Select(ContentResultUtil.Render);
  }

  private IObservable<CurrencyDto> HandleDuplicateEntryCheck(CurrencyDto dto)
  {
    return _currencyDomainHandler.SelectCurrencyExistsUsingCode(dto.CurrencyCode)
      .Select(existingCurrency =>
      {
        if (existingCurrency)
        {
          Logger.Warn($"WriteCurrenciesHandler@HandleDuplicateEntryCheck: Currency with code {dto.CurrencyCode} already exists.");
          throw new StandardException($"Currency with code {dto.CurrencyCode} already exists.");
        }
        return dto;
      });
  }
}
