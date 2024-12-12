using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<ProductModel?>;

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ProductModel?>
{
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;

  public DeleteProductCommandHandler(IProductQuerier productQuerier, IProductRepository productRepository)
  {
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

    product.Delete(actorId: null); // TODO(fpion): provide actor ID

    // TODO(fpion): ensure no cart references this product
    await _productRepository.SaveAsync(product, cancellationToken);

    return model;
  }
}
