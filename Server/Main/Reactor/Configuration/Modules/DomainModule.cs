using Server.Main.Reactor.Handlers.Domain;

namespace Server.Main.Reactor.Configuration.Modules;

public static class DomainModule
{
  public static void Configure(IServiceCollection services)
  {
    services.AddSingleton<CurrencyDomainHandler>();
    services.AddSingleton<FinancialProductDomainHandler>();
    services.AddSingleton<SubscriptionDomainHandler>();
    services.AddSingleton<TransactionDomainHandler>();
    services.AddSingleton<TransactionDetailsDomainHandler>();
  }
}
