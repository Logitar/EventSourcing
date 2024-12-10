using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Kurrent;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingWithKurrent(this IServiceCollection services, string connectionString)
  {
    EventStoreClientSettings settings = EventStoreClientSettings.Create(connectionString);

    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddSingleton<IEventConverter, EventConverter>()
      .AddScoped(_ => new EventStoreClient(settings))
      .AddScoped<IEventStore, KurrentStore>();
  }
}
