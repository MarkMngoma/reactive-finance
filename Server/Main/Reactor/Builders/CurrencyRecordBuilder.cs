namespace Server.Main.Reactor.Builders;

public class CurrencyRecordBuilder
{
  private int? _currencyId;
  private string? _currencyCode;
  private string? _currencySymbol;
  private string? _currencyFlag;
  private string? _currencyName;

  public CurrencyRecordBuilder WithCurrencyId(int currencyId)
  {
    _currencyId = currencyId;
    return this;
  }

  public CurrencyRecordBuilder WithCurrencyCode(string currencyCode)
  {
    _currencyCode = currencyCode;
    return this;
  }

  public CurrencyRecordBuilder WithCurrencySymbol(string currencySymbol)
  {
    _currencySymbol = currencySymbol;
    return this;
  }

  public CurrencyRecordBuilder WithCurrencyFlag(string currencyFlag)
  {
    _currencyFlag = currencyFlag;
    return this;
  }

  public CurrencyRecordBuilder WithCurrencyName(string currencyName)
  {
    _currencyName = currencyName;
    return this;
  }

  public object Build()
  {
    return new
    {
      CURRENCY_ID = _currencyId,
      CURRENCY_CODE = _currencyCode,
      CURRENCY_SYMBOL = _currencySymbol,
      CURRENCY_FLAG = _currencyFlag,
      CURRENCY_NAME = _currencyName,
      ARCHIVED = 0,
      CREATED_AT = DateTimeOffset.Now,
      CREATED_BY = 1
    };
  }
}
