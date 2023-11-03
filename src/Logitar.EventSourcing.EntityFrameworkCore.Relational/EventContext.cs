using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// The database context for events.
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
  /// Gets or sets the data set of events.
  /// </summary>
  public DbSet<EventEntity> Events { get; private set; } = null!;

  /// <summary>
  /// Configures the specified model builder.
  /// </summary>
  /// <param name="modelBuilder">The model builder.</param>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventContext).Assembly);
  }
}
