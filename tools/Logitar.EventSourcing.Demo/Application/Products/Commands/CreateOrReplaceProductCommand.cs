using FluentValidation;
using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Application.Products.Validators;
using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.Domain.Products;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Products.Commands;

public record CreateOrReplaceProductResult(ProductModel? Product = null, bool Created = false);

public record CreateOrReplaceProductCommand(Guid? Id, ProductPayload Payload, long? Version) : IRequest<CreateOrReplaceProductResult>;

internal class CreateOrReplaceProductCommandHandler : IRequestHandler<CreateOrReplaceProductCommand, CreateOrReplaceProductResult>
{
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;

  public CreateOrReplaceProductCommandHandler(IProductQuerier productQuerier, IProductRepository productRepository)
  {
    _productQuerier = productQuerier;
    _productRepository = productRepository;
  }

  public async Task<CreateOrReplaceProductResult> Handle(CreateOrReplaceProductCommand command, CancellationToken cancellationToken)
  {
    ProductPayload payload = command.Payload;
    new ProductValidator().ValidateAndThrow(payload);

    bool created = false;
    ProductId? id = null;
    Product? product = null;
    if (command.Id.HasValue)
    {
      id = new(command.Id.Value);
      product = await _productRepository.LoadAsync(id.Value, cancellationToken);
    }

    Sku sku = new(payload.Sku);
    if (product == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceProductResult();
      }

      product = new(sku, id: id);
      created = true;
    }

    Product reference = (command.Version.HasValue
      ? await _productRepository.LoadAsync(product.Id, command.Version.Value, cancellationToken)
      : null) ?? product;

    if (reference.Sku != sku)
    {
      product.Sku = sku;
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      product.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      product.Description = description;
    }

    Price? price = Price.TryCreate(payload.Price);
    if (reference.Price != price)
    {
      product.Price = price;
    }
    Url? pictureUrl = Url.TryCreate(payload.PictureUrl);
    if (reference.PictureUrl != pictureUrl)
    {
      product.PictureUrl = pictureUrl;
    }

    product.Update(actorId: null);

    // TODO(fpion): enforce SKU unicity
    await _productRepository.SaveAsync(product, cancellationToken);

    ProductModel model = await _productQuerier.ReadAsync(product, cancellationToken);
    return new CreateOrReplaceProductResult(model, created);
  }
}
