using Logitar.Identity.ApiKeys.Commands;
using Logitar.Identity.ApiKeys.Queries;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Implements methods to manage API keys in the identity system.
/// </summary>
internal class ApiKeyService : IApiKeyService
{
  /// <summary>
  /// The request pipeline.
  /// </summary>
  private readonly IRequestPipeline _requestPipeline;

  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyService"/> class using the specified arguments.
  /// </summary>
  /// <param name="requestPipeline">The request pipeline.</param>
  public ApiKeyService(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  /// <summary>
  /// Authenticates an API key.
  /// </summary>
  /// <param name="apiKey">The string representation of the API key.</param>
  /// <param name="prefix">The expected API key prefix.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The authenticated API key.</returns>
  public async Task<ApiKey> AuthenticateAsync(string apiKey, string prefix, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new AuthenticateApiKeyCommand(apiKey, prefix), cancellationToken);
  }

  /// <summary>
  /// Creates a new API key.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created API key.</returns>
  public async Task<ApiKey> CreateAsync(CreateApiKeyInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new CreateApiKeyCommand(input), cancellationToken);
  }

  /// <summary>
  /// Deletes an API key.
  /// </summary>
  /// <param name="id">The identifier of the API key.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted API key.</returns>
  public async Task<ApiKey> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DeleteApiKeyCommand(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves an API key by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the API key.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The API key, or null if not found.</returns>
  public async Task<ApiKey?> GetAsync(Guid? id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetApiKeyQuery(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a list of API keys using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm to filter by.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of API keys to skip.</param>
  /// <param name="take">The number of API keys to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The API key list, or empty if none found.</returns>
  public async Task<PagedList<ApiKey>> GetAsync(string? realm, string? search, ApiKeySort? sort, bool isDescending,
    int? skip, int? take, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetApiKeysQuery(realm, search,
      sort, isDescending, skip, take), cancellationToken);
  }

  /// <summary>
  /// Updates an API key.
  /// </summary>
  /// <param name="id">The identifier of the API key.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated API key.</returns>
  public async Task<ApiKey> UpdateAsync(Guid id, UpdateApiKeyInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new UpdateApiKeyCommand(id, input), cancellationToken);
  }
}
