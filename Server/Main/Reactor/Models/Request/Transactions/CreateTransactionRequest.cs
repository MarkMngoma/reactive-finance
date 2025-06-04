using Server.Main.Reactor.Models.Enums;

namespace Server.Main.Reactor.Models.Request.Transactions;

public class CreateTransactionRequest
{
  public decimal Amount { get; set; }

  public string Type { get; set; } = TransactionType.Purchase.ToString();

  public string Status { get; set; } = TransactionStatus.Pending.ToString();

  public string? Description { get; set; }

  public required string SubscriptionId { get; set; }
}
