using Logitar.EventSourcing.Demo.Domain.Carts;

namespace Logitar.EventSourcing.Demo.Application.Carts;

public interface ICartRepository
{
  Task<Cart?> LoadAsync(CartId id, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Cart>> LoadAsync(IEnumerable<CartId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Cart cart, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Cart> carts, CancellationToken cancellationToken = default);
}
