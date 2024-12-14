using Logitar.EventSourcing.Demo.Application.Carts;
using Logitar.EventSourcing.Demo.Domain.Carts;

namespace Logitar.EventSourcing.Demo.Infrastructure.Repositories;

internal class CartRepository : Repository, ICartRepository
{
  public CartRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Cart?> LoadAsync(CartId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Cart>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Cart>> LoadAsync(IEnumerable<CartId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<Cart>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Cart cart, CancellationToken cancellationToken)
  {
    await base.SaveAsync(cart, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Cart> carts, CancellationToken cancellationToken)
  {
    await base.SaveAsync(carts, cancellationToken);
  }
}
