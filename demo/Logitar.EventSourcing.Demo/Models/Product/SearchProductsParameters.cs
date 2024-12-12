using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Application.Search;
using Logitar.EventSourcing.Demo.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.EventSourcing.Demo.Models.Product;

public record SearchProductsParameters : SearchParameters
{
  [FromQuery(Name = "price_from")]
  public decimal? PriceFrom { get; set; }

  [FromQuery(Name = "price_under")]
  public decimal? PriceUnder { get; set; }

  public SearchProductsPayload ToPayload()
  {
    SearchProductsPayload payload = new()
    {
      PriceFrom = PriceFrom,
      PriceUnder = PriceUnder
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ProductSort field))
      {
        payload.Sort.Add(new ProductSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
