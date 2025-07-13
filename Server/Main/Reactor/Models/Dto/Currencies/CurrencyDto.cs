namespace Server.Main.Reactor.Models.Dto.Currencies;

public class CurrencyDto
{
  public int CurrencyId { get; set; }
  public required string? CurrencyCode { get; set; }
  public required string CurrencyName { get; set; }
  public required string CurrencySymbol { get; set; }
  public required string CurrencyFlag { get; set; }
}
