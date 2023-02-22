using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// The database context used to fetch and save events.
/// </summary>
public class EventContext : DbContext
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventContext"/> class using the specified options.
  /// </summary>
  /// <param name="options">The database context options.</param>
  public EventContext(DbContextOptions<EventContext> options) : base(options)
  {
  }

  /// <summary>
  /// The data set of delete actions.
  /// </summary>
  public DbSet<DeleteActionEntity> DeleteActions { get; private set; } = null!;
  /// <summary>
  /// The data set of events.
  /// </summary>
  public DbSet<EventEntity> Events { get; private set; } = null!;

  /// <summary>
  /// Configures the database context model creation process using the specified model builder.
  /// </summary>
  /// <param name="builder">The model builder.</param>
  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(typeof(EventContext).Assembly);
    builder.HasPostgresExtension("uuid-ossp");
  }
}
