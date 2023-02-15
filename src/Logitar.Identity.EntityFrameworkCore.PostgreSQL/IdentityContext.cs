using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// The database context used for the identity system.
/// </summary>
public class IdentityContext : DbContext
{
  /// <summary>
  /// Initializes a new instance of the <see cref="IdentityContext"/> class using the specified options.
  /// </summary>
  /// <param name="options">The database context options.</param>
  public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
  {
  }

  /// <summary>
  /// Gets or sets the data set of realms.
  /// </summary>
  internal DbSet<RealmEntity> Realms { get; private set; } = null!;
  /// <summary>
  /// Gets or sets the data set of roles.
  /// </summary>
  internal DbSet<RoleEntity> Roles { get; private set; } = null!;

  /// <summary>
  /// Configures the database context model creation process using the specified model builder.
  /// </summary>
  /// <param name="builder">The model builder.</param>
  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
  }
}
