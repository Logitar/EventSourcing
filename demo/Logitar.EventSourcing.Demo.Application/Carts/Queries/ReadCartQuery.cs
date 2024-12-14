using Logitar.EventSourcing.Demo.Application.Carts.Models;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Carts.Queries;

public record ReadCartQuery(Guid Id) : IRequest<CartModel?>;

internal class ReadCartQueryHandler : IRequestHandler<ReadCartQuery, CartModel?>
{
  private readonly ICartQuerier _cartQuerier;

  public ReadCartQueryHandler(ICartQuerier cartQuerier)
  {
    _cartQuerier = cartQuerier;
  }

  public async Task<CartModel?> Handle(ReadCartQuery query, CancellationToken cancellationToken)
  {
    return await _cartQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
