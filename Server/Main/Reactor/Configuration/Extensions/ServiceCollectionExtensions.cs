using Microsoft.Extensions.Options;

namespace Server.Main.Reactor.Configuration.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddConfig<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class
  {
    services.Configure<T>(configuration.GetSection(sectionName));
    services.AddSingleton(sp => sp.GetRequiredService<IOptions<T>>().Value);
    return services;
  }
}
