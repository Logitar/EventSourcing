using Logitar.EventSourcing.Demo.Application.Carts;
using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Domain.Carts;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<ProductModel?>;

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ProductModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ICartQuerier _cartQuerier;
  private readonly ICartRepository _cartRepository;
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;

  public DeleteProductCommandHandler(
    IApplicationContext applicationContext,
    ICartQuerier cartQuerier,
    ICartRepository cartRepository,
    IProductQuerier productQuerier,
    IProductRepository productRepository)
  {
    _applicationContext = applicationContext;
    _cartQuerier = cartQuerier;
    _cartRepository = cartRepository;
    _productQuerier = productQuerier;
    _productRepository = productRepository;
  }

  public async Task<ProductModel?> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
  {
    ProductId id = new(command.Id);
    Product? product = await _productRepository.LoadAsync(id, cancellationToken);
    if (product == null)
    {
      return null;
    }
    ProductModel model = await _productQuerier.ReadAsync(product, cancellationToken);

    ActorId? actorId = _applicationContext.ActorId;

    await RemoveFromCartsAsync(product, actorId, cancellationToken);

    product.Delete(actorId);

    await _productRepository.SaveAsync(product, cancellationToken);

    return model;
  }

  private async Task RemoveFromCartsAsync(Product product, ActorId? actorId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<CartId> cartIds = await _cartQuerier.FindIdsAsync(product.Id, cancellationToken);
    if (cartIds.Count == 0)
    {
      return;
    }

    IReadOnlyCollection<Cart> carts = await _cartRepository.LoadAsync(cartIds, cancellationToken);
    if (carts.Count == 0)
    {
      return;
    }

    foreach (Cart cart in carts)
    {
      cart.RemoveItem(product.Id, quantity: int.MaxValue, actorId);
    }

    await _cartRepository.SaveAsync(carts, cancellationToken);
  }
}
