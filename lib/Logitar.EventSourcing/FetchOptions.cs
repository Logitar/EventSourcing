namespace Logitar.EventSourcing;

/// <summary>
/// Represents options for fetching a stream of events.
/// </summary>
public record FetchOptions
{
  /// <summary>
  /// Gets or sets the starting event version (inclusive).
  /// </summary>
  public long FromVersion { get; set; }
  /// <summary>
  /// Gets or sets the ending event version (inclusive).
  /// </summary>
  public long ToVersion { get; set; }
}
