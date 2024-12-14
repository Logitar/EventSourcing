using Logitar.EventSourcing.Demo.Domain.Carts.Events;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.Infrastructure.Handlers;

internal class CartEvents : INotificationHandler<CartCreated>,
  INotificationHandler<CartItemAdded>,
  INotificationHandler<CartItemRemoved>,
  INotificationHandler<CartItemUpdated>
{
  private readonly DemoContext _context;

  public CartEvents(DemoContext context)
  {
    _context = context;
  }

  public async Task Handle(CartCreated @event, CancellationToken cancellationToken)
  {
    CartEntity? cart = await _context.Carts.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.StreamId.Value, cancellationToken);
    if (cart == null)
    {
      cart = new(@event);

      _context.Carts.Add(cart);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(CartItemAdded @event, CancellationToken cancellationToken)
  {
    CartEntity cart = await _context.Carts
      .Include(x => x.Items)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The cart entity 'AggregateId={@event.StreamId}' could not be found.");

    ProductEntity product = await _context.Products
      .SingleOrDefaultAsync(x => x.AggregateId == @event.ProductId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The product entity 'AggregateId={@event.ProductId}' could not be found.");

    cart.AddItem(product, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(CartItemRemoved @event, CancellationToken cancellationToken)
  {
    CartEntity cart = await _context.Carts
      .Include(x => x.Items)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The cart entity 'AggregateId={@event.StreamId}' could not be found.");

    ProductEntity product = await _context.Products
      .SingleOrDefaultAsync(x => x.AggregateId == @event.ProductId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The product entity 'AggregateId={@event.ProductId}' could not be found.");

    cart.RemoveItem(product, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(CartItemUpdated @event, CancellationToken cancellationToken)
  {
    CartEntity cart = await _context.Carts
      .Include(x => x.Items)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The cart entity 'AggregateId={@event.StreamId}' could not be found.");

    ProductEntity product = await _context.Products
      .SingleOrDefaultAsync(x => x.AggregateId == @event.ProductId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The product entity 'AggregateId={@event.ProductId}' could not be found.");

    cart.UpdateItem(product, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
