namespace Server.Main.Reactor.Models.Request.Transactions;

public class CreateTransactionRequest
{
  public decimal Amount { get; set; }
  public string? Type { get; set; }
}
