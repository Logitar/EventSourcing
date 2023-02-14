using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddEventSourcingWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
    {
      return services
        .AddDbContext<EventContext>(options => options.UseNpgsql(connectionString))
        .AddScoped<IEventStore, EventStore>();
    }
  }
}
