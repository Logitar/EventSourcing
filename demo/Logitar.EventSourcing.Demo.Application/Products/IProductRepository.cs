using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Application.Products;

public interface IProductRepository
{
  Task<Product?> LoadAsync(ProductId id, CancellationToken cancellationToken = default);
  Task<Product?> LoadAsync(ProductId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Product product, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);
}
