using Logitar.EventSourcing.Demo.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.Demo.Infrastructure.Configurations;

internal class CartConfiguration : AggregateConfiguration<CartEntity>, IEntityTypeConfiguration<CartEntity>
{
  public override void Configure(EntityTypeBuilder<CartEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(DemoDb.Carts.Table.Table ?? string.Empty, DemoDb.Carts.Table.Schema);
    builder.HasKey(x => x.CartId);
  }
}
