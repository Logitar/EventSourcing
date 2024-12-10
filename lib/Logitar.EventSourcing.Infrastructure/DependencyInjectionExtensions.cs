using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingInfrastructure(this IServiceCollection services)
  {
    return services.AddSingleton<IEventSerializer, EventSerializer>();
  }
}
