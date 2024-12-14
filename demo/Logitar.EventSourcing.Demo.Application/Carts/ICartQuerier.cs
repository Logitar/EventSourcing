using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Domain.Carts;

namespace Logitar.EventSourcing.Demo.Application.Carts;

public interface ICartQuerier
{
  Task<CartModel> ReadAsync(Cart cart, CancellationToken cancellationToken = default);
  Task<CartModel?> ReadAsync(CartId id, CancellationToken cancellationToken = default);
  Task<CartModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
