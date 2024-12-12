using Logitar.EventSourcing.Demo.Application.Products;
using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Infrastructure.Repositories;

internal class ProductRepository : Repository, IProductRepository
{
  public ProductRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Product?> LoadAsync(ProductId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Product?> LoadAsync(ProductId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Product>(id.StreamId, version, cancellationToken);
  }

  public async Task SaveAsync(Product product, CancellationToken cancellationToken)
  {
    await base.SaveAsync(product, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Product> products, CancellationToken cancellationToken)
  {
    await base.SaveAsync(products, cancellationToken);
  }
}
