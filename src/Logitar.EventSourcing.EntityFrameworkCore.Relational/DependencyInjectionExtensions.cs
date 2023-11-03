using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Provides extension methods for Dependency Injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies for Event Sourcing using the Relational Entity Framework Core store in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcingWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddScoped<IAggregateRepository, AggregateRepository>();
  }
}
