namespace Server.Main.Reactor.Models.Enums;

public enum TransactionStatus
{
  Pending,
  Authorized,
  Settled,
  Failed,
  Cancelled,
  Reversed
}

