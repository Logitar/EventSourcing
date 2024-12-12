using Logitar.EventSourcing.Demo.Domain.Products.Events;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.Infrastructure.Handlers;

internal class ProductEvents : INotificationHandler<ProductCreated>, INotificationHandler<ProductDeleted>, INotificationHandler<ProductUpdated>
{
  private readonly DemoContext _context;

  public ProductEvents(DemoContext context)
  {
    _context = context;
  }

  public async Task Handle(ProductCreated @event, CancellationToken cancellationToken)
  {
    ProductEntity? product = await _context.Products.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.StreamId.Value, cancellationToken);
    if (product == null)
    {
      product = new(@event);

      _context.Products.Add(product);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(ProductDeleted @event, CancellationToken cancellationToken)
  {
    ProductEntity? product = await _context.Products
      .SingleOrDefaultAsync(x => x.AggregateId == @event.StreamId.Value, cancellationToken);
    if (product != null)
    {
      _context.Products.Remove(product);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(ProductUpdated @event, CancellationToken cancellationToken)
  {
    ProductEntity product = await _context.Products
      .SingleOrDefaultAsync(x => x.AggregateId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The product entity 'AggregateId={@event.StreamId}' could not be found.");

    product.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
