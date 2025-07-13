using Server.Main.Reactor.Models.Enums;

namespace Server.Main.Reactor.Models.Dto.Transactions;

public class UpdateTransactionDto
{
  public string? TransactionId { get; set; }

  public string Status { get; set; } = TransactionStatus.Pending.ToString();

}
