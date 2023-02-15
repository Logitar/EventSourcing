using Logitar.Identity.Realms;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Identity;

/// <summary>
/// Provides extension methods to use the identity system.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies of the identity system to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddLogitarIdentity(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddMediatR(assembly)
      .RegisterFacades();
  }

  /// <summary>
  /// Registers the service facades to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  private static IServiceCollection RegisterFacades(this IServiceCollection services)
  {
    return services.AddTransient<IRealmService, RealmService>();
  }
}
