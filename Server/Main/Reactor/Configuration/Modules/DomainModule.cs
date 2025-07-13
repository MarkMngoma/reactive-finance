using Server.Main.Reactor.Handlers.Domain;

namespace Server.Main.Reactor.Configuration.Modules;

public static class DomainModule
{
  public static void Configure(IServiceCollection services)
  {
    services.AddSingleton<ICurrencyDomainHandler, CurrencyDomainHandler>();
    services.AddSingleton<IFinancialProductDomainHandler, FinancialProductDomainHandler>();
    services.AddSingleton<ISubscriptionDomainHandler, SubscriptionDomainHandler>();
    services.AddSingleton<ITransactionDomainHandler, TransactionDomainHandler>();
    services.AddSingleton<ITransactionDetailsDomainHandler, TransactionDetailsDomainHandler>();
  }
}
