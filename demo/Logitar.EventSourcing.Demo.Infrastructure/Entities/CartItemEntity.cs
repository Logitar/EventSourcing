using Logitar.EventSourcing.Demo.Domain.Carts.Events;

namespace Logitar.EventSourcing.Demo.Infrastructure.Entities;

internal class CartItemEntity
{
  public CartEntity? Cart { get; private set; }
  public int CartId { get; private set; }

  public ProductEntity? Product { get; private set; }
  public int ProductId { get; private set; }

  public int Quantity { get; private set; }

  public CartItemEntity(CartEntity cart, ProductEntity product, CartItemAdded @event) : this(cart, product, @event.Quantity)
  {
  }
  public CartItemEntity(CartEntity cart, ProductEntity product, CartItemUpdated @event) : this(cart, product, @event.Quantity)
  {
  }
  private CartItemEntity(CartEntity cart, ProductEntity product, int quantity)
  {
    Cart = cart;
    CartId = cart.CartId;

    Product = product;
    ProductId = product.ProductId;

    Quantity = quantity;
  }

  private CartItemEntity()
  {
  }

  public void Add(int quantity)
  {
    Quantity += quantity;
  }

  public void Remove(int quantity)
  {
    Quantity -= quantity;
  }

  public void Update(int quantity)
  {
    Quantity = quantity;
  }
}
