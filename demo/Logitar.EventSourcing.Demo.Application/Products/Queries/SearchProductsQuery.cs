using Logitar.EventSourcing.Demo.Application.Products.Models;
using Logitar.EventSourcing.Demo.Application.Search;
using MediatR;

namespace Logitar.EventSourcing.Demo.Application.Products.Queries;

public record SearchProductsQuery(SearchProductsPayload Payload) : IRequest<SearchResults<ProductModel>>;

internal class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, SearchResults<ProductModel>>
{
  private readonly IProductQuerier _productQuerier;

  public SearchProductsQueryHandler(IProductQuerier productQuerier)
  {
    _productQuerier = productQuerier;
  }

  public async Task<SearchResults<ProductModel>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
  {
    return await _productQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
