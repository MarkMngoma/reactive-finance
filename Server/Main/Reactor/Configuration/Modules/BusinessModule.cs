using Server.Main.Reactor.Handlers.Business.Finance;
using Server.Main.Reactor.Handlers.Business.Subscriptions;
using Server.Main.Reactor.Handlers.Business.Transactions;

namespace Server.Main.Reactor.Configuration.Modules;

public static class BusinessModule
{
  public static void Configure(IServiceCollection services)
  {
    services.AddSingleton<WriteCurrenciesHandler>();
    services.AddSingleton<WriteBatchCurrenciesHandler>();
    services.AddSingleton<QueryCurrenciesHandler>();
    services.AddSingleton<QueryFinancialProductHandler>();
    services.AddSingleton<CreateAdhocTransactionHandler>();
    services.AddSingleton<CreateRefundTransactionHandler>();
    services.AddSingleton<QueryRefundEligibilityHandler>();
    services.AddSingleton<QueryTransactionHistoryHandler>();
    services.AddSingleton<SettlementHandler>();
    services.AddSingleton<CreateSubscriptionHandler>();
    services.AddSingleton<UpdateSubscriptionHandler>();
    services.AddSingleton<DeleteSubscriptionHandler>();
  }
}

