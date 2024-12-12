using FluentValidation;
using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Application.Products.Validators;
using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Products.Commands;

public record UpdateProductCommand(Guid Id, UpdateProductPayload Payload) : IRequest<ProductModel?>;

internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductModel?>
{
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;

  public UpdateProductCommandHandler(IProductQuerier productQuerier, IProductRepository productRepository)
  {
    _productQuerier = productQuerier;
    _productRepository = productRepository;
  }

  public async Task<ProductModel?> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
  {
    UpdateProductPayload payload = command.Payload;
    new UpdateProductValidator().ValidateAndThrow(payload);

    ProductId id = new(command.Id);
    Product? product = await _productRepository.LoadAsync(id, cancellationToken);
    if (product == null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.Sku))
    {
      product.Sku = new Sku(payload.Sku);
    }
    if (payload.DisplayName != null)
    {
      product.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      product.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Price != null)
    {
      product.Price = Price.TryCreate(payload.Price.Value);
    }
    if (payload.PictureUrl != null)
    {
      product.PictureUrl = Url.TryCreate(payload.PictureUrl.Value);
    }

    product.Update(actorId: null);

    // TODO(fpion): enforce SKU unicity
    await _productRepository.SaveAsync(product, cancellationToken);

    return await _productQuerier.ReadAsync(product, cancellationToken);
  }
}
