namespace Logitar.EventSourcing;

/// <summary>
/// Represents options for fetching a stream of events.
/// </summary>
public record FetchStreamOptions
{
  /// <summary>
  /// Gets or sets the starting event version (inclusive).
  /// </summary>
  public long FromVersion { get; set; }
  /// <summary>
  /// Gets or sets the ending event version (inclusive).
  /// </summary>
  public long ToVersion { get; set; }

  /// <summary>
  /// Gets or sets the actor filter.
  /// </summary>
  public ActorFilter? Actor { get; set; }

  /// <summary>
  /// Gets or sets the starting event date and time (inclusive).
  /// </summary>
  public DateTime? OccurredFrom { get; set; }
  /// <summary>
  /// Gets or sets the starting event date and time (inclusive).
  /// </summary>
  public DateTime? OccurredTo { get; set; }

  /// <summary>
  /// Gets or sets the stream deletion status filter.
  /// </summary>
  public bool? IsDeleted { get; set; }
}
