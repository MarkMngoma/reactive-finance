using Src.Main.Reactor.Models.Dto;

namespace Src.Main.Infrastructure.Builders;

public class InsertCurrencyRecordBuilder
{
  private readonly CurrencyDto _request;

  public InsertCurrencyRecordBuilder(CurrencyDto request)
  {
    _request = request;
  }

  public object Build()
  {
    return new
    {
      CURRENCY_ID = _request.CurrencyId,
      CURRENCY_CODE = _request.CurrencyCode,
      CURRENCY_SYMBOL = _request.CurrencySymbol,
      CURRENCY_FLAG = _request.CurrencyFlag,
      CURRENCY_NAME = _request.CurrencyName,
      ARCHIVED = 0,
      CREATED_AT = DateTimeOffset.Now,
      CREATED_BY = 1
    };
  }
}
