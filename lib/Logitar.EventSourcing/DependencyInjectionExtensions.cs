using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing;

/// <summary>
/// Provides extension methods for Dependency Injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies for Event Sourcing framework in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcing(this IServiceCollection services)
  {
    return services.AddScoped<IRepository, Repository>();
  }
}
