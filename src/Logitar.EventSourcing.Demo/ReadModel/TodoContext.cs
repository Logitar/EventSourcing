using Logitar.EventSourcing.Demo.ReadModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.ReadModel;

internal class TodoContext : DbContext
{
  public TodoContext(DbContextOptions<TodoContext> options) : base(options)
  {
  }

  public DbSet<TodoEntity> Todos { get; private set; } = null!;

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(typeof(TodoContext).Assembly);
  }
}
