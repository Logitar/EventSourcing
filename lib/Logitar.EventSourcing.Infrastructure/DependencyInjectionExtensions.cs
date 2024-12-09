using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Provides extension methods for Dependency Injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies for Event Sourcing infrastructure layer in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcingInfrastructure(this IServiceCollection services)
  {
    return services.AddSingleton<IEventSerializer, EventSerializer>();
  }
}
