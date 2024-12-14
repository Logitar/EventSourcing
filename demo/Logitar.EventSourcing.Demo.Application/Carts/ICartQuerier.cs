using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Domain.Carts;
using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Application.Carts;

public interface ICartQuerier
{
  Task<IReadOnlyCollection<CartId>> FindIdsAsync(ProductId productId, CancellationToken cancellationToken = default);

  Task<CartModel> ReadAsync(Cart cart, CancellationToken cancellationToken = default);
  Task<CartModel?> ReadAsync(CartId id, CancellationToken cancellationToken = default);
  Task<CartModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
