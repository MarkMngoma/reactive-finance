
using System.Text.Json.Serialization;

namespace Src.Main.Reactor.Models.Dto;

public class ExchangeRatesDto
{
  [JsonPropertyName("date")]
  public required string Date { get; set; }

  [JsonPropertyName("eur")]
  public required Dictionary<string, decimal> Eur { get; set; }
}
