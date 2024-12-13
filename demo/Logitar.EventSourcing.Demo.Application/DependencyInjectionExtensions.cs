using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Demo.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingDemoApplication(this IServiceCollection services)
  {
    return services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }
}
