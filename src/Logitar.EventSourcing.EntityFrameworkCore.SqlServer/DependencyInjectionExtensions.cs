using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.EntityFrameworkCore.SqlServer;

/// <summary>
/// Provides extension methods for Dependency Injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies for Event Sourcing using the SQL Server Entity Framework Core store in the specified dependency container.
  /// </summary>
  /// <param name="services">The dependency container.</param>
  /// <param name="connectionString">The connection string to the SQL Server database.</param>
  /// <returns>The dependency container.</returns>
  public static IServiceCollection AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<EventContext>(options => options.UseSqlServer(connectionString,
        options => options.MigrationsAssembly("Logitar.EventSourcing.EntityFrameworkCore.SqlServer")
      ))
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational();
  }
}
