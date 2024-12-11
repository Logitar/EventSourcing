using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Provides extension methods for Dependency Injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies for Event Sourcing using the PostgreSQL Entity Framework Core store in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <param name="connectionString">The connection string to the PostgreSQL database.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<EventContext>(options => options.UseNpgsql(connectionString,
        options => options.MigrationsAssembly("Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL")
      ))
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational();
  }
}
