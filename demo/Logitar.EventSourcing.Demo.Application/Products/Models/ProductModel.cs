namespace Logitar.EventSourcing.Demo.Application.Products.Models;

public class ProductModel : AggregateModel
{
  public new Guid Id
  {
    get => Guid.Parse(base.Id);
    set => base.Id = value.ToString();
  }

  public string Sku { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public decimal? Price { get; set; }
  public string? PictureUrl { get; set; }

  public override string ToString() => $"{DisplayName ?? Sku} | {base.ToString()}";
}
