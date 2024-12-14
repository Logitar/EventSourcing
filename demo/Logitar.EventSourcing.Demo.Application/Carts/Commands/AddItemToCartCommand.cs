using FluentValidation;
using Logitar.EventSourcing.Demo.Application.Carts.Models;
using Logitar.EventSourcing.Demo.Application.Carts.Validators;
using Logitar.EventSourcing.Demo.Application.Products;
using Logitar.EventSourcing.Demo.Domain.Carts;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Carts.Commands;

public record AddItemToCartResult(CartModel Cart, bool Created);

public record AddItemToCartCommand(Guid? CartId, Guid ProductId, QuantityPayload Payload) : IRequest<AddItemToCartResult>;

internal class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, AddItemToCartResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ICartQuerier _cartQuerier;
  private readonly ICartRepository _cartRepository;
  private readonly IProductRepository _productRepository;

  public AddItemToCartCommandHandler(
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

  public async Task<AddItemToCartResult> Handle(AddItemToCartCommand command, CancellationToken cancellationToken)
  {
    QuantityPayload payload = command.Payload;
    new QuantityValidator().ValidateAndThrow(payload);

    ActorId? actorId = _applicationContext.ActorId;
    int quantity = payload.Quantity < 1 ? 1 : payload.Quantity;

    bool created = false;
    CartId? cartId = null;
    Cart? cart = null;
    if (command.CartId.HasValue)
    {
      cartId = new(command.CartId.Value);
      cart = await _cartRepository.LoadAsync(cartId.Value, cancellationToken);
    }
    if (cart == null)
    {
      cart = new(actorId, cartId);
      created = true;
    }

    ProductId productId = new(command.ProductId);
    Product product = await _productRepository.LoadAsync(productId, cancellationToken)
      ?? throw new ProductNotFoundException(productId, nameof(command.ProductId));

    cart.AddItem(product, quantity, actorId);

    await _cartRepository.SaveAsync(cart, cancellationToken);

    CartModel model = await _cartQuerier.ReadAsync(cart, cancellationToken);
    return new AddItemToCartResult(model, created);
  }
}
