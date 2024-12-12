using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<ProductModel?>;

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ProductModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;

  public DeleteProductCommandHandler(IApplicationContext applicationContext, IProductQuerier productQuerier, IProductRepository productRepository)
  {
    _applicationContext = applicationContext;
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

    product.Delete(_applicationContext.ActorId);

    // TODO(fpion): ensure no cart references this product
    await _productRepository.SaveAsync(product, cancellationToken);

    return model;
  }
}
