namespace Server.Main.Reactor.Configuration.Objects;

public class ForexConfig
{
  public required string BaseCurrency { get; set; }
  public required string SupportedCurrencies { get; set; }
  public required string ExchangeRatesApiUrl { get; set; }
}
