namespace Server.Main.Reactor.Models.Dto.Subscriptions;

public class CreateSubscriptionDto
{
  public int SubscriptionType { get; set; }
  public DateTime BillingDate { get; set; }
  public double RecurringAmount { get; set; }
  public int Frequency { get; set; }
  public int Cycles { get; set; }
  public bool SubscriptionNotifyEmail { get; set; }
  public bool SubscriptionNotifyWebhook { get; set; }
  public bool SubscriptionNotifyBuyer { get; set; }
}
