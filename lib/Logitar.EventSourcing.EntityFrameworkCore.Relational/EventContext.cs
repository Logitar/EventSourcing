using Logitar.EventSourcing.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

public class EventContext : DbContext
{
  public EventContext(DbContextOptions<EventContext> options) : base(options)
  {
  }

  public DbSet<EventEntity> Events => Set<EventEntity>();
  public DbSet<StreamEntity> Streams => Set<StreamEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
