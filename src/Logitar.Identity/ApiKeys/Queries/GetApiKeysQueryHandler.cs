using MediatR;

namespace Logitar.Identity.ApiKeys.Queries;

/// <summary>
/// The handler for the <see cref="GetApiKeysQuery"/> queries.
/// </summary>
internal class GetApiKeysQueryHandler : IRequestHandler<GetApiKeysQuery, PagedList<ApiKey>>
{
  /// <summary>
  /// The API key querier.
  /// </summary>
  private readonly IApiKeyQuerier _apiKeyQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetApiKeysQueryHandler"/> class.
  /// </summary>
  /// <param name="apiKeyQuerier">The API key querier.</param>
  public GetApiKeysQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of API keys, or a empty collection.</returns>
  public async Task<PagedList<ApiKey>> Handle(GetApiKeysQuery request, CancellationToken cancellationToken)
  {
    return await _apiKeyQuerier.GetAsync(request.Realm, request.Search, request.Sort, request.IsDescending,
      request.Skip, request.Take, cancellationToken);
  }
}
