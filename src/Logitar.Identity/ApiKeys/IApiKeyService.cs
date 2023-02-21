namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Exposes methods to manage API keys in the identity system.
/// </summary>
public interface IApiKeyService
{
  /// <summary>
  /// Authenticates an API key.
  /// </summary>
  /// <param name="apiKey">The string representation of the API key.</param>
  /// <param name="prefix">The expected API key prefix.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The authenticated API key.</returns>
  Task<ApiKey> AuthenticateAsync(string apiKey, string prefix, CancellationToken cancellationToken = default);
  /// <summary>
  /// Creates a new API key.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created API key.</returns>
  Task<ApiKey> CreateAsync(CreateApiKeyInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes an API key.
  /// </summary>
  /// <param name="id">The identifier of the API key.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted API key.</returns>
  Task<ApiKey> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves an API key by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the API key.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The API key, or null if not found.</returns>
  Task<ApiKey?> GetAsync(Guid? id = null, CancellationToken cancellationToken = default);
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
  /// <returns>The list of API keys, or empty if none found.</returns>
  Task<PagedList<ApiKey>> GetAsync(string? realm = null, string? search = null, ApiKeySort? sort = null, bool isDescending = false,
    int? skip = null, int? take = null, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates an API key.
  /// </summary>
  /// <param name="id">The identifier of the API key.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated API key.</returns>
  Task<ApiKey> UpdateAsync(Guid id, UpdateApiKeyInput input, CancellationToken cancellationToken = default);
}
