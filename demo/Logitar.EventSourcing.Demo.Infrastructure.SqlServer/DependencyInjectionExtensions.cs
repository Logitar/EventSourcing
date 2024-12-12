using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Demo.Infrastructure.SqlServer;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingDemoInfrastructureWithSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddLogitarEventSourcingDemoInfrastructure()
      .AddDbContext<DemoContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Logitar.EventSourcing.Demo.Infrastructure.SqlServer")))
      .AddSingleton<ISqlHelper, SqlServerHelper>();
  }
}
