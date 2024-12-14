using Logitar.EventSourcing.Demo.Domain.Products.Events;

namespace Logitar.EventSourcing.Demo.Infrastructure.Entities;

internal class ProductEntity : AggregateEntity
{
  public int ProductId { get; private set; }
  public Guid Id { get; private set; }

  public string Sku { get; private set; } = string.Empty;
  public string SkuNormalized
  {
    get => DemoDb.Helper.Normalize(Sku);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public decimal? Price { get; private set; }
  public string? PictureUrl { get; private set; }

  public List<CartItemEntity> CartItems { get; private set; } = [];

  public ProductEntity(ProductCreated @event) : base(@event)
  {
    Id = @event.StreamId.ToGuid();

    Sku = @event.Sku.Value;
  }

  private ProductEntity() : base()
  {
  }

  public void Update(ProductUpdated @event)
  {
    base.Update(@event);

    if (@event.Sku != null)
    {
      Sku = @event.Sku.Value;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Price != null)
    {
      Price = @event.Price.Value?.Value;
    }
    if (@event.PictureUrl != null)
    {
      PictureUrl = @event.PictureUrl.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? Sku} | {base.ToString()}";
}
