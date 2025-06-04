using Server.Main.Reactor.Models.Request;

namespace Server.Main.Reactor.Builders;

public class InsertCurrencyRecordBuilder
{
  private readonly CurrencyRequest _request;

  public InsertCurrencyRecordBuilder(CurrencyRequest request)
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
