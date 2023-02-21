namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Represents the possible sort values for API keys.
/// </summary>
public enum ApiKeySort
{
  /// <summary>
  /// The API keys will be sorted by their expiration date and time.
  /// </summary>
  ExpiresOn,

  /// <summary>
  /// The API keys will be sorted by their title (or display name).
  /// </summary>
  Title,

  /// <summary>
  /// The API keys will be sorted by their latest update date and time, including creation.
  /// </summary>
  UpdatedOn
}
