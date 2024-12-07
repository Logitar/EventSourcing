namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Represents the metadata of an event.
/// </summary>
public record EventMetadata
{
  /// <summary>
  /// Gets the type of the event stream.
  /// </summary>
  public string? StreamType { get; }
  /// <summary>
  /// Gets the type of the event.
  /// </summary>
  public string EventType { get; }
  /// <summary>
  /// Gets the version of the event.
  /// </summary>
  public long Version { get; }

  /// <summary>
  /// Gets the identifier of the actor who raised the event.
  /// </summary>
  public string? ActorId { get; }
  /// <summary>
  /// Gets the date and time when the event was raised.
  /// </summary>
  public DateTime OccurredOn { get; }
  /// <summary>
  /// Gets a value indicating whether or not the stream is deleted, undeleted or unchanged.
  /// </summary>
  public bool? IsDeleted { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventMetadata"/> class.
  /// </summary>
  /// <param name="streamType">The stream type.</param>
  /// <param name="eventType">The event type.</param>
  /// <param name="version">The event version.</param>
  /// <param name="actorId">The identifier of the actor who raised the event.</param>
  /// <param name="occurredOn">The date and time when the event was raised.</param>
  /// <param name="isDeleted">A value indicating whether or not the stream is deleted, undeleted or unchanged.</param>
  public EventMetadata(string? streamType, string eventType, long version, string? actorId, DateTime occurredOn, bool? isDeleted)
  {
    StreamType = streamType;
    EventType = eventType;
    Version = version;

    ActorId = actorId;
    OccurredOn = occurredOn;
    IsDeleted = isDeleted;
  }
}
