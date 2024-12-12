using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Application.Search;
using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Application.Products;

public interface IProductQuerier
{
  Task<ProductModel> ReadAsync(Product product, CancellationToken cancellationToken = default);
  Task<ProductModel?> ReadAsync(ProductId id, CancellationToken cancellationToken = default);
  Task<ProductModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ProductModel?> ReadAsync(string sku, CancellationToken cancellationToken = default);

  Task<SearchResults<ProductModel>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken = default);
}
