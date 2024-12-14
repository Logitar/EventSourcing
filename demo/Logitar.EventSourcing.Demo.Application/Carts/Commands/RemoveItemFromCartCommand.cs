using FluentValidation;
using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Application.Carts.Validators;
using Logitar.EventSourcing.Demo.Domain.Carts;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Carts.Commands;

public record RemoveItemFromCartCommand(Guid CartId, Guid ProductId, QuantityPayload Payload) : IRequest<CartModel?>;

internal class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, CartModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ICartQuerier _cartQuerier;
  private readonly ICartRepository _cartRepository;

  public RemoveItemFromCartCommandHandler(IApplicationContext applicationContext, ICartQuerier cartQuerier, ICartRepository cartRepository)
  {
    _applicationContext = applicationContext;
    _cartQuerier = cartQuerier;
    _cartRepository = cartRepository;
  }

  public async Task<CartModel?> Handle(RemoveItemFromCartCommand command, CancellationToken cancellationToken)
  {
    QuantityPayload payload = command.Payload;
    new QuantityValidator().ValidateAndThrow(payload);

    CartId cartId = new(command.CartId);
    Cart? cart = await _cartRepository.LoadAsync(cartId, cancellationToken);
    if (cart == null)
    {
      return null;
    }

    ProductId productId = new(command.ProductId);
    int quantity = payload.Quantity > 0 ? payload.Quantity : int.MaxValue;
    cart.RemoveItem(productId, quantity, _applicationContext.ActorId);

    await _cartRepository.SaveAsync(cart, cancellationToken);

    return await _cartQuerier.ReadAsync(cart, cancellationToken);
  }
}
