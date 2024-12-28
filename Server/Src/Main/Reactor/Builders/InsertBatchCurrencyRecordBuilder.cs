using Src.Main.Reactor.Models.Dto;
using static Src.Main.Reactor.Builders.Tables.Generated.CurrenciesTable;

namespace Src.Main.Reactor.Builders;

public class InsertBatchCurrencyRecordBuilder
{
  private readonly List<CurrencyDto> _currencies = [];

  public InsertBatchCurrencyRecordBuilder AddCurrencyEntry(BatchCurrencyDto dto)
  {
    dto.BatchCurrencies.ForEach(item =>
    {
      _currencies.Add(new CurrencyDto
      {
        CurrencyId = item.CurrencyId,
        CurrencyCode = item.CurrencyCode,
        CurrencySymbol = item.CurrencySymbol,
        CurrencyFlag = item.CurrencyFlag,
        CurrencyName = item.CurrencyName,
      });
    });

    return this;
  }

  public (IReadOnlyList<string> Columns, List<object[]> Records) Build()
  {
    var records = _currencies.Select(entity => new object[]
    {
      entity.CurrencyId,
      entity.CurrencyCode,
      entity.CurrencySymbol,
      entity.CurrencyFlag,
      entity.CurrencyName,
      0,
      DateTimeOffset.Now,
      1
    }).ToList();

    return (Columns, records);
  }

  private IReadOnlyList<string> Columns { get; } = new List<string>
  {
    CurrencyId,
    CurrencyCode,
    CurrencySymbol,
    CurrencyFlag,
    CurrencyName,
    Archived,
    CreatedAt,
    CreatedBy
  };
}
