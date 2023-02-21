namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Represents a cached API key.
/// </summary>
/// <param name="Aggregate">The API key aggregate.</param>
/// <param name="Output">The API key output representation.</param>
public record CachedApiKey(ApiKeyAggregate Aggregate, ApiKey Output);
