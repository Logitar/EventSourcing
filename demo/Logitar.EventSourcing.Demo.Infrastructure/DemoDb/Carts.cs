using Logitar.Data;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;

namespace Logitar.EventSourcing.Demo.Infrastructure.DemoDb;

internal static class Carts
{
  public static readonly TableId Table = new(nameof(DemoContext.Carts));

  public static readonly ColumnId CartId = new(nameof(CartEntity.CartId), Table);
  public static readonly ColumnId Id = new(nameof(CartEntity.Id), Table);
}
