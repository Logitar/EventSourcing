using Logitar.EventSourcing.Demo.Application;
using Logitar.EventSourcing.Demo.Application.Carts;
using Logitar.EventSourcing.Demo.Application.Products;
using Logitar.EventSourcing.Demo.Infrastructure.Queriers;
using Logitar.EventSourcing.Demo.Infrastructure.Repositories;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Demo.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarEventSourcingDemoInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingDemoApplication()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddScoped<IEventBus, EventBus>()
      .AddQueriers()
      .AddRepositories();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<ICartQuerier, CartQuerier>()
      .AddScoped<IProductQuerier, ProductQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<ICartRepository, CartRepository>()
      .AddScoped<IProductRepository, ProductRepository>();
  }
}
