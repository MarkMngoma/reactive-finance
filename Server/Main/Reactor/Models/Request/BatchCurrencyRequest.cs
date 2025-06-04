namespace Server.Main.Reactor.Models.Request;

public class BatchCurrencyRequest
{
  public required List<CurrencyRequest> BatchCurrencies { get; set; }
}
