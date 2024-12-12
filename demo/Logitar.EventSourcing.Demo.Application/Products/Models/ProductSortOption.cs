using Logitar.EventSourcing.Demo.Application.Search;

namespace Logitar.EventSourcing.Demo.Application.Products.Models;

public record ProductSortOption : SortOption
{
  public new ProductSort Field
  {
    get => Enum.Parse<ProductSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ProductSortOption() : this(ProductSort.DisplayText)
  {
  }

  public ProductSortOption(ProductSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
