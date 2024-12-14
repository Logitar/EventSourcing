using Logitar.EventSourcing.Demo.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.Demo.Infrastructure.Configurations;

internal class CartItemConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
  public void Configure(EntityTypeBuilder<CartItemEntity> builder)
  {
    builder.ToTable(DemoDb.CartItems.Table.Table ?? string.Empty, DemoDb.CartItems.Table.Schema);
    builder.HasKey(x => new { x.CartId, x.ProductId });

    builder.HasIndex(x => x.Quantity);

    builder.HasOne(x => x.Cart).WithMany(x => x.Items)
      .HasPrincipalKey(x => x.CartId).HasForeignKey(x => x.CartId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Product).WithMany(x => x.CartItems)
      .HasPrincipalKey(x => x.ProductId).HasForeignKey(x => x.ProductId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
