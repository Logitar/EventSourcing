using Logitar.EventSourcing.Demo.Domain.Products.Events;

namespace Logitar.EventSourcing.Demo.Domain.Products;

public class Product : AggregateRoot
{
  private ProductUpdated _updated = new();

  public new ProductId Id => new(base.Id);

  private Sku? _sku = null;
  public Sku Sku
  {
    get => _sku ?? throw new InvalidOperationException($"The {nameof(Sku)} has not been initialized yet.");
    set
    {
      if (_sku != value)
      {
        _sku = value;
        _updated.Sku = value;
      }
    }
  }
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  private Price? _price = null;
  public Price? Price
  {
    get => _price;
    set
    {
      if (_price != value)
      {
        _price = value;
        _updated.Price = new Change<Price>(value);
      }
    }
  }
  private Url? _pictureUrl = null;
  public Url? PictureUrl
  {
    get => _pictureUrl;
    set
    {
      if (_pictureUrl != value)
      {
        _pictureUrl = value;
        _updated.PictureUrl = new Change<Url>(value);
      }
    }
  }

  public Product() : base(ProductId.NewId().StreamId)
  {
  }

  public Product(Sku sku, ActorId? actorId = null, ProductId? id = null) : base((id ?? ProductId.NewId()).StreamId)
  {
    Raise(new ProductCreated(sku), actorId);
  }
  protected virtual void Handle(ProductCreated @event)
  {
    _sku = @event.Sku;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ProductDeleted(), actorId);
    }
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(ProductUpdated @event)
  {
    if (@event.Sku != null)
    {
      _sku = @event.Sku;
    }
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Price != null)
    {
      _price = @event.Price.Value;
    }
    if (@event.PictureUrl != null)
    {
      _pictureUrl = @event.PictureUrl.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? Sku.Value} | {base.ToString()}";
}
