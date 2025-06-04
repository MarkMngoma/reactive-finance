using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Server.Main.Reactor.Models.Request;

[UsedImplicitly]
public class ExchangeRatesRequest
{
  [JsonPropertyName("date")]
  public required string Date { get; set; }

  [JsonPropertyName("eur")]
  public required Dictionary<string, decimal> Eur { get; set; }
}
