using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Provides extension methods for Dependency Injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies for Event Sourcing using the EventStoreDB/Kurrent store in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <param name="connectionString">The connection string to the EventStoreDB/Kurrent database.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcingWithKurrent(this IServiceCollection services, string connectionString)
  {
    EventStoreClientSettings settings = EventStoreClientSettings.Create(connectionString);

    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddSingleton<IEventConverter, EventConverter>()
      .AddScoped<IEventStore, KurrentEventStore>()
      .AddScoped(_ => new EventStoreClient(settings));
  }
}
