using FluentValidation;
using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Application.Carts.Validators;
using Logitar.EventSourcing.Demo.Application.Products;
using Logitar.EventSourcing.Demo.Domain.Carts;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Carts.Commands;

public record UpdateCartItemCommand(Guid CartId, Guid ProductId, QuantityPayload Payload) : IRequest<CartModel?>;

internal class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ICartQuerier _cartQuerier;
  private readonly ICartRepository _cartRepository;
  private readonly IProductRepository _productRepository;

  public UpdateCartItemCommandHandler(
    IApplicationContext applicationContext,
    ICartQuerier cartQuerier,
    ICartRepository cartRepository,
    IProductRepository productRepository)
  {
    _applicationContext = applicationContext;
    _cartQuerier = cartQuerier;
    _cartRepository = cartRepository;
    _productRepository = productRepository;
  }

  public async Task<CartModel?> Handle(UpdateCartItemCommand command, CancellationToken cancellationToken)
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
    Product product = await _productRepository.LoadAsync(productId, cancellationToken)
      ?? throw new ProductNotFoundException(productId, nameof(command.ProductId));

    cart.SetItem(product, payload.Quantity, _applicationContext.ActorId);

    await _cartRepository.SaveAsync(cart, cancellationToken);

    return await _cartQuerier.ReadAsync(cart, cancellationToken);
  }
}
