using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Provides extension methods to use the EntityFrameworkCore PostgreSQL event store.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies to the specified service collection using the specified connection string.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddEventSourcingWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<EventContext>(options => options.UseNpgsql(connectionString))
      .AddScoped<IEventStore, EventStore>();
  }
}
