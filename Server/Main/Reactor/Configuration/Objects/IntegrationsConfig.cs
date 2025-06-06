namespace Server.Main.Reactor.Configuration.Objects;

public class PayFast
{
  public required string MerchantId { get; set; }
  public required string MerchantKey { get; set; }
  public required string Passphrase { get; set; }
  public required string JsonApiBaseUrl { get; set; }
  public required string FormApiBaseUrl { get; set; }
  public required string ReturnUrl { get; set; }
  public required string CancelUrl { get; set; }
  public required string NotifyUrl { get; set; }
}
public class IntegrationsConfig
{
  public required PayFast PayFast { get; set; }
}
