namespace Logitar.EventSourcing.Demo.Application.Products.Models;

public record ProductPayload
{
  public string Sku { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public decimal? Price { get; set; }
  public string? PictureUrl { get; set; }
}
