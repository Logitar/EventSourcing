using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

public class EventContext : DbContext
{
  public EventContext(DbContextOptions<EventContext> options) : base(options)
  {
  }

  public DbSet<DeleteActionEntity> DeleteActions { get; private set; } = null!;
  public DbSet<EventEntity> Events { get; private set; } = null!;

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(typeof(EventContext).Assembly);
    builder.HasPostgresExtension("uuid-ossp");
  }
}
