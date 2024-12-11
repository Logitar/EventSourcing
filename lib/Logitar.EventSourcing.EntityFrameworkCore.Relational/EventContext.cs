using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents the database context for events.
/// </summary>
public class EventContext : DbContext
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventContext"/> class.
  /// </summary>
  /// <param name="options">The database context options.</param>
  public EventContext(DbContextOptions<EventContext> options) : base(options)
  {
  }

  /// <summary>
  /// Gets the data set of event streams.
  /// </summary>
  public DbSet<StreamEntity> Streams => Set<StreamEntity>();

  /// <summary>
  /// Configures the specified model builder.
  /// </summary>
  /// <param name="modelBuilder">The model builder.</param>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
