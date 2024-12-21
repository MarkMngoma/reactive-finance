using Newtonsoft.Json;

namespace Server.Models.Dto;

public class ExchangeRatesDto
{
  [JsonProperty("date")]
  public required string Date { get; set; }

  [JsonProperty("eur")]
  public required Dictionary<string, decimal> Eur { get; set; }
}
