using Logitar.EventSourcing.Demo.Domain.Carts.Events;
using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Domain.Carts;

public class Cart : AggregateRoot
{
  public new CartId Id => new(base.Id);

  private readonly Dictionary<ProductId, int> _items = [];
  public IReadOnlyDictionary<ProductId, int> Items => _items.AsReadOnly();

  public Cart() : base(CartId.NewId().StreamId)
  {
  }

  public Cart(ActorId? actorId = null, CartId? id = null) : base((id ?? CartId.NewId()).StreamId)
  {
    Raise(new CartCreated(), actorId);
  }

  public void AddItem(Product product, int quantity, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
    Raise(new CartItemAdded(product.Id, quantity), actorId);
  }
  protected virtual void Handle(CartItemAdded @event)
  {
    _ = _items.TryGetValue(@event.ProductId, out int existingQuantity);
    _items[@event.ProductId] = existingQuantity + @event.Quantity;
  }

  public void RemoveItem(ProductId productId, int quantity, ActorId? actorId = null)
  {
    if (_items.TryGetValue(productId, out int existingQuantity))
    {
      Raise(new CartItemRemoved(productId, quantity > existingQuantity ? existingQuantity : quantity), actorId);
    }
  }
  protected virtual void Handle(CartItemRemoved @event)
  {
    _ = _items.TryGetValue(@event.ProductId, out int existingQuantity);
    if (@event.Quantity >= existingQuantity)
    {
      _items.Remove(@event.ProductId);
    }
    else
    {
      _items[@event.ProductId] = existingQuantity - @event.Quantity;
    }
  }

  public void SetItem(Product product, int quantity, ActorId? actorId = null)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));
    Raise(new CartItemUpdated(product.Id, quantity), actorId);
  }
  protected virtual void Handle(CartItemUpdated @event)
  {
    if (@event.Quantity < 1)
    {
      _items.Remove(@event.ProductId);
    }
    else
    {
      _items[@event.ProductId] = @event.Quantity;
    }
  }
}
