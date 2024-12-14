using Logitar.EventSourcing.Demo.Domain.Carts.Events;

namespace Logitar.EventSourcing.Demo.Infrastructure.Entities;

internal class CartEntity : AggregateEntity
{
  public int CartId { get; private set; }
  public Guid Id { get; private set; }

  public List<CartItemEntity> Items { get; private set; } = [];

  public CartEntity(CartCreated @event) : base(@event)
  {
    Id = @event.StreamId.ToGuid();
  }

  private CartEntity() : base()
  {
  }

  public void AddItem(ProductEntity product, CartItemAdded @event)
  {
    base.Update(@event);

    CartItemEntity? item = Items.SingleOrDefault(i => i.ProductId == product.ProductId);
    if (item == null)
    {
      item = new(this, product, @event);
      Items.Add(item);
    }
    else
    {
      item.Add(@event.Quantity);
    }
  }

  public void RemoveItem(ProductEntity product, CartItemRemoved @event)
  {
    base.Update(@event);

    CartItemEntity? item = Items.SingleOrDefault(i => i.ProductId == product.ProductId);
    if (item != null)
    {
      if (@event.Quantity >= item.Quantity)
      {
        Items.Remove(item);
      }
      else
      {
        item.Remove(@event.Quantity);
      }
    }
  }

  public void UpdateItem(ProductEntity product, CartItemUpdated @event)
  {
    base.Update(@event);

    CartItemEntity? item = Items.SingleOrDefault(i => i.ProductId == product.ProductId);
    if (item == null)
    {
      if (@event.Quantity > 0)
      {
        item = new(this, product, @event);
        Items.Add(item);
      }
    }
    else if (@event.Quantity > 0)
    {
      item.Update(@event.Quantity);
    }
    else
    {
      Items.Remove(item);
    }
  }
}
