using Logitar.EventSourcing;
using Logitar.Identity.ApiKeys;

namespace Logitar.Identity;

/// <summary>
/// Defines methods to store and retrieve cached informations.
/// </summary>
public interface ICacheService
{
  /// <summary>
  /// Retrieves the cached API key with the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier of the API key.</param>
  /// <returns>The cached API key or null if not found.</returns>
  CachedApiKey? GetApiKey(AggregateId id);
  /// <summary>
  /// Stores an API key in the cache.
  /// </summary>
  /// <param name="aggregate">The API key aggregate.</param>
  /// <param name="output">The API key output representation.</param>
  void SetApiKey(ApiKeyAggregate aggregate, ApiKey output);
}
