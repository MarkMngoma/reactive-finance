using Server.Main.Reactor.Builders.Tables.Generated.Models;

namespace Server.Main.Reactor.Builders;

public class CurrencyRecordBuilder
{
    private readonly CurrenciesDto _dto = new();

    public CurrencyRecordBuilder WithCurrencyId(int currencyId)
    {
        _dto.CurrencyId = currencyId;
        return this;
    }

    public CurrencyRecordBuilder WithCurrencyCode(string currencyCode)
    {
        _dto.CurrencyCode = currencyCode;
        return this;
    }

    public CurrencyRecordBuilder WithCurrencySymbol(string currencySymbol)
    {
        _dto.CurrencySymbol = currencySymbol;
        return this;
    }

    public CurrencyRecordBuilder WithCurrencyFlag(string currencyFlag)
    {
        _dto.CurrencyFlag = currencyFlag;
        return this;
    }

    public CurrencyRecordBuilder WithCurrencyName(string currencyName)
    {
        _dto.CurrencyName = currencyName;
        return this;
    }

    public CurrencyRecordBuilder WithDefaults()
    {
        _dto.Archived = false;
        _dto.CreatedAt = DateTimeOffset.Now.UtcDateTime;
        _dto.CreatedBy = 1u;
        return this;
    }

    public CurrenciesDto Build()
    {
        return _dto;
    }

}
