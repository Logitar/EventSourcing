using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.InMemory;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingInMemory(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddSingleton<IAggregateRepository, AggregateRepository>();
  }
}
