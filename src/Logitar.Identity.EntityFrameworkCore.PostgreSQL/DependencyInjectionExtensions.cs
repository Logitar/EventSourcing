using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Repositories;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using Logitar.Identity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Provides extension methods to use the EntityFrameworkCore PostgreSQL identity store.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers the required dependencies to the specified service collection using the specified connection string.
  /// </summary>
  /// <param name="services">The service collection</param>
  /// <param name="connectionString">The connection string</param>
  /// <returns>The service collection</returns>
  public static IServiceCollection AddLogitarIdentityWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
      .AddDbContext<IdentityContext>(options => options.UseNpgsql(connectionString))
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddQueriers()
      .AddRepositories();
  }

  /// <summary>
  /// Registers the queriers to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection</param>
  /// <returns>The service collection</returns>
  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IAddressQuerier, AddressQuerier>()
      .AddScoped<IEmailQuerier, EmailQuerier>()
      .AddScoped<IPhoneQuerier, PhoneQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>()
      .AddScoped<IRoleQuerier, RoleQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }

  /// <summary>
  /// Registers the repositories to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection</param>
  /// <returns>The service collection</returns>
  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IAddressRepository, AddressRepository>()
      .AddScoped<IEmailRepository, EmailRepository>()
      .AddScoped<IPhoneRepository, PhoneRepository>()
      .AddScoped<IRealmRepository, RealmRepository>()
      .AddScoped<IRoleRepository, RoleRepository>()
      .AddScoped<IUserRepository, UserRepository>();
  }
}
