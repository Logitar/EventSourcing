using Logitar.EventSourcing.Demo.Application;
using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;

namespace Logitar.EventSourcing.Demo.Infrastructure;

internal static class Mapper
{
  public static CartModel ToCartModel(CartEntity source)
  {
    CartModel destination = new();

    foreach (CartItemEntity item in source.Items)
    {
      if (item.Product == null)
      {
        throw new InvalidOperationException($"The {nameof(item.Product)} is required.");
      }

      destination.Items.Add(new ItemModel
      {
        Product = ToProductModel(item.Product),
        Quantity = item.Quantity
      });
    }

    MapAggregate(source, destination);
    destination.Id = source.Id;

    return destination;
  }

  public static ProductModel ToProductModel(ProductEntity source)
  {
    ProductModel destination = new()
    {
      Sku = source.Sku,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Price = source.Price,
      PictureUrl = source.PictureUrl
    };

    MapAggregate(source, destination);
    destination.Id = source.Id;

    return destination;
  }

  private static void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Id = source.AggregateId;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
    destination.Version = source.Version;
  }
}
