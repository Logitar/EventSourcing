using Logitar.EventSourcing.Demo.Application;
using Logitar.EventSourcing.Demo.Application.Products;
using Logitar.EventSourcing.Demo.Infrastructure.Queriers;
using Logitar.EventSourcing.Demo.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Demo.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingDemoInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingDemoApplication()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddScoped<IProductQuerier, ProductQuerier>()
      .AddScoped<IProductRepository, ProductRepository>();
  }
}
