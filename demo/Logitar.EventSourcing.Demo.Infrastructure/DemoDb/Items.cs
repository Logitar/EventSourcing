using Logitar.Data;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;

namespace Logitar.EventSourcing.Demo.Infrastructure.DemoDb;

internal static class CartItems
{
  public static readonly TableId Table = new(nameof(DemoContext.CartItems));

  public static readonly ColumnId CartId = new(nameof(CartItemEntity.CartId), Table);
  public static readonly ColumnId ProductId = new(nameof(CartItemEntity.ProductId), Table);
  public static readonly ColumnId Quantity = new(nameof(CartItemEntity.Quantity), Table);
}
