namespace Server.Main.Reactor.Models.Request.Subscriptions;

public class UpdateSubscriptionRequest
{
  public required string SubscriptionId { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public required string ProductId { get; set; }
  public int BillingCycle { get; set; } = 3;
}
