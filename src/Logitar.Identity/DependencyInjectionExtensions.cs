using Logitar.Identity.Contacts;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using Logitar.Identity.Users;
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
      .AddFacades()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddTransient<IPasswordHelper, PasswordHelper>()
      .AddTransient<IUserHelper, UserHelper>();
  }

  /// <summary>
  /// Registers the service facades to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IAddressService, AddressService>()
      .AddTransient<IEmailService, EmailService>()
      .AddTransient<IPhoneService, PhoneService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<IRoleService, RoleService>()
      .AddTransient<IUserService, UserService>();
  }
}
