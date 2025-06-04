using Server.Main.Reactor.Models.Enums;

namespace Server.Main.Reactor.Models.Request.Transactions;

public class UpdateTransactionRequest
{
  public string? TransactionId { get; set; }

  public string Status { get; set; } = TransactionStatus.Pending.ToString();

}
