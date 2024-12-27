using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Demo.Infrastructure.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingDemoInfrastructureWithPostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<DemoContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Logitar.EventSourcing.Demo.Infrastructure.PostgreSQL")))
      .AddSingleton<ISqlHelper, PostgresHelper>();
  }
}
