using Logitar.Data;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;

namespace Logitar.EventSourcing.Demo.Infrastructure.DemoDb;

internal static class Products
{
  public static readonly TableId Table = new(nameof(DemoContext.Products));

  public static readonly ColumnId Description = new(nameof(ProductEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(ProductEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(ProductEntity.Id), Table);
  public static readonly ColumnId PictureUrl = new(nameof(ProductEntity.PictureUrl), Table);
  public static readonly ColumnId Price = new(nameof(ProductEntity.Price), Table);
  public static readonly ColumnId ProductId = new(nameof(ProductEntity.ProductId), Table);
  public static readonly ColumnId Sku = new(nameof(ProductEntity.Sku), Table);
  public static readonly ColumnId SkuNormalized = new(nameof(ProductEntity.SkuNormalized), Table);
}
