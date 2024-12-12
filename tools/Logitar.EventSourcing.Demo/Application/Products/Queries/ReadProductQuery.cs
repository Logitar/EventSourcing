using Logitar.EventSourcing.Demo.Application.Products.Models;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Products.Queries;

public record ReadProductQuery(Guid? Id, string? Sku) : IRequest<ProductModel?>;

internal class ReadProductQueryHandler : IRequestHandler<ReadProductQuery, ProductModel?>
{
  private readonly IProductQuerier _productQuerier;

  public ReadProductQueryHandler(IProductQuerier productQuerier)
  {
    _productQuerier = productQuerier;
  }

  public async Task<ProductModel?> Handle(ReadProductQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ProductModel> products = new(capacity: 2);

    if (query.Id.HasValue)
    {
      ProductModel? product = await _productQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (product != null)
      {
        products[product.Id] = product;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Sku))
    {
      ProductModel? product = await _productQuerier.ReadAsync(query.Sku, cancellationToken);
      if (product != null)
      {
        products[product.Id] = product;
      }
    }

    if (products.Count > 1)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }

    return products.SingleOrDefault().Value;
  }
}
