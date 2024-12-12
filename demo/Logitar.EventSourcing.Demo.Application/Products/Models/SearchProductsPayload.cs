using Logitar.EventSourcing.Demo.Application.Search;

namespace Logitar.EventSourcing.Demo.Application.Products.Models;

public record SearchProductsPayload : SearchPayload
{
  public decimal PriceFrom { get; set; }
  public decimal PriceUnder { get; set; }

  public new List<ProductSortOption> Sort { get; set; } = [];
}
