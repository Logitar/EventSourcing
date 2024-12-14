using Logitar.EventSourcing.Demo.Application.Carts;
using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Domain.Carts;
using Logitar.EventSourcing.Demo.Domain.Products;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.Infrastructure.Queriers;

internal class CartQuerier : ICartQuerier
{
  private readonly DbSet<CartEntity> _carts;

  public CartQuerier(DemoContext context)
  {
    _carts = context.Carts;
  }

  public async Task<IReadOnlyCollection<CartId>> FindIdsAsync(ProductId productId, CancellationToken cancellationToken)
  {
    string[] aggregateIds = await _carts.AsNoTracking()
      .Include(x => x.Items).ThenInclude(x => x.Product)
      .Where(x => x.Items.Any(item => item.Product!.Id == productId.ToGuid()))
      .Select(x => x.AggregateId)
      .ToArrayAsync(cancellationToken);

    return aggregateIds.Select(value => new CartId(value)).ToArray();
  }

  public async Task<CartModel> ReadAsync(Cart cart, CancellationToken cancellationToken)
  {
    return await ReadAsync(cart.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The cart entity 'AggregateId={cart.Id}' could not be found.");
  }
  public async Task<CartModel?> ReadAsync(CartId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<CartModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CartEntity? cart = await _carts.AsNoTracking()
      .Include(x => x.Items).ThenInclude(x => x.Product)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return cart == null ? null : Mapper.ToCartModel(cart);
  }
}
