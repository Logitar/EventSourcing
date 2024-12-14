using Logitar.EventSourcing.Demo.Application.Products.Models;

namespace Logitar.EventSourcing.Demo.Application.Carts.Models;

public record ItemModel
{
  public ProductModel Product { get; set; } = new();
  public int Quantity { get; set; }
}
