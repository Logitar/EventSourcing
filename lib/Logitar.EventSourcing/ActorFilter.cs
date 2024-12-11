namespace Logitar.EventSourcing;

/// <summary>
/// Represents a filter to match on event actor identifiers.
/// </summary>
public record ActorFilter
{
  /// <summary>
  /// Gets or sets the value of the filter.
  /// </summary>
  public ActorId? ActorId { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ActorFilter"/> class.
  /// </summary>
  public ActorFilter()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ActorFilter"/> class.
  /// </summary>
  /// <param name="actorId">The value of the filter.</param>
  public ActorFilter(ActorId? actorId)
  {
    ActorId = actorId;
  }
}
