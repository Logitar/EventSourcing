using Logitar.EventSourcing;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Exposes methods used to query API key read models.
/// </summary>
public interface IApiKeyQuerier
{
  /// <summary>
  /// Retrieves a API key by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The API key or null if not found.</returns>
  Task<ApiKey?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a API key by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The API key or null if not found.</returns>
  Task<ApiKey?> GetAsync(Guid id, CancellationToken cancellationToken = default);
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
}
