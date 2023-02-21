using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.ApiKeys.Queries;

/// <summary>
/// The handler for <see cref="GetApiKeyQuery"/> queries.
/// </summary>
internal class GetApiKeyQueryHandler : IRequestHandler<GetApiKeyQuery, ApiKey?>
{
  /// <summary>
  /// The API key querier.
  /// </summary>
  private readonly IApiKeyQuerier _apiKeyQuerier;

  /// <summary>
  /// Initializes a new instance of the <see cref="GetApiKeyQueryHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="apiKeyQuerier">The API key querier.</param>
  public GetApiKeyQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  /// <summary>
  /// Handles the specified query instance.
  /// </summary>
  /// <param name="request">The query to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved API key or null.</returns>
  /// <exception cref="TooManyResultsException">More than one API keys have been found.</exception>
  public async Task<ApiKey?> Handle(GetApiKeyQuery request, CancellationToken cancellationToken)
  {
    List<ApiKey> apiKeys = new(capacity: 2);

    if (request.Id.HasValue)
    {
      apiKeys.AddIfNotNull(await _apiKeyQuerier.GetAsync(request.Id.Value, cancellationToken));
    }

    if (apiKeys.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return apiKeys.SingleOrDefault();
  }
}
