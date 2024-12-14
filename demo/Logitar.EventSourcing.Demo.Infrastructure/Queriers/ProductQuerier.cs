using Logitar.Data;
using Logitar.EventSourcing.Demo.Application.Products;
using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Application.Search;
using Logitar.EventSourcing.Demo.Domain.Products;
using Logitar.EventSourcing.Demo.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.Infrastructure.Queriers;

internal class ProductQuerier : IProductQuerier
{
  private readonly DbSet<ProductEntity> _products;
  private readonly ISqlHelper _sqlHelper;

  public ProductQuerier(DemoContext context, ISqlHelper sqlHelper)
  {
    _products = context.Products;
    _sqlHelper = sqlHelper;
  }

  public async Task<ProductId?> FindIdAsync(Sku sku, CancellationToken cancellationToken)
  {
    string skuNormalized = DemoDb.Helper.Normalize(sku.Value);

    string? aggregateId = await _products.AsNoTracking()
      .Where(x => x.SkuNormalized == skuNormalized)
      .Select(x => x.AggregateId)
      .SingleOrDefaultAsync(cancellationToken);

    return aggregateId == null ? null : new ProductId(aggregateId);
  }

  public async Task<ProductModel> ReadAsync(Product product, CancellationToken cancellationToken)
  {
    return await ReadAsync(product.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The product entity 'AggregateId={product.Id}' could not be found.");
  }
  public async Task<ProductModel?> ReadAsync(ProductId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<ProductModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ProductEntity? product = await _products.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return product == null ? null : Mapper.ToProductModel(product);
  }
  public async Task<ProductModel?> ReadAsync(string sku, CancellationToken cancellationToken)
  {
    string skuNormalized = DemoDb.Helper.Normalize(sku);

    ProductEntity? product = await _products.AsNoTracking()
      .SingleOrDefaultAsync(x => x.SkuNormalized == skuNormalized, cancellationToken);

    return product == null ? null : Mapper.ToProductModel(product);
  }

  public async Task<SearchResults<ProductModel>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(DemoDb.Products.Table).SelectAll(DemoDb.Products.Table)
      .ApplyIdFilter(payload, DemoDb.Products.Id);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, DemoDb.Products.Sku, DemoDb.Products.DisplayName);

    if (payload.PriceFrom.HasValue)
    {
      builder.Where(DemoDb.Products.Price, Operators.IsGreaterThanOrEqualTo(payload.PriceFrom.Value));
    }
    if (payload.PriceUnder.HasValue)
    {
      builder.Where(DemoDb.Products.Price, Operators.IsLessThan(payload.PriceUnder.Value));
    }

    IQueryable<ProductEntity> query = _products.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ProductEntity>? ordered = null;
    foreach (ProductSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ProductSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ProductSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ProductSort.DisplayText:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName ?? x.Sku) : query.OrderBy(x => x.DisplayName ?? x.Sku))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName ?? x.Sku) : ordered.ThenBy(x => x.DisplayName ?? x.Sku));
          break;
        case ProductSort.Price:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Price) : ordered.ThenBy(x => x.Price));
          break;
        case ProductSort.Sku:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Sku) : query.OrderBy(x => x.Sku))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Sku) : ordered.ThenBy(x => x.Sku));
          break;
        case ProductSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    ProductEntity[] products = await query.ToArrayAsync(cancellationToken);
    IEnumerable<ProductModel> items = products.Select(Mapper.ToProductModel);

    return new SearchResults<ProductModel>(items, total);
  }
}
