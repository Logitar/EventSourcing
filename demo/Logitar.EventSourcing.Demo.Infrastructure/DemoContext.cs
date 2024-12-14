using Logitar.EventSourcing.Demo.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.Infrastructure;

public class DemoContext : DbContext
{
  public DemoContext(DbContextOptions<DemoContext> options) : base(options)
  {
  }

  internal DbSet<CartEntity> Carts => Set<CartEntity>();
  internal DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
  internal DbSet<ProductEntity> Products => Set<ProductEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
