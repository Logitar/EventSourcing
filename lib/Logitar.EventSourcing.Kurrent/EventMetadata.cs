namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Represents the metadata of an event.
/// </summary>
public record EventMetadata
{
  /// <summary>
  /// Gets the type of the event.
  /// </summary>
  public Type EventType { get; }
  /// <summary>
  /// Gets the identifier of the event.
  /// </summary>
  public EventId? EventId { get; }
  /// <summary>
  /// Gets the type of the stream in which the event resides.
  /// </summary>
  public Type? StreamType { get; }
  /// <summary>
  /// Gets the version of the event.
  /// </summary>
  public long? Version { get; }
  /// <summary>
  /// Gets the identifier of the actor who raised the event.
  /// </summary>
  public ActorId? ActorId { get; }
  /// <summary>
  /// Gets the date and time when the event occurred.
  /// </summary>
  public DateTime? OccurredOn { get; }
  /// <summary>
  /// Gets a value indicating whether or not the event deletes, undeletes or leaves unchanged its stream deletion status.
  /// </summary>
  public bool? IsDeleted { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventMetadata"/> class.
  /// </summary>
  /// <param name="eventType">The event type.</param>
  /// <param name="eventId">The event identifier.</param>
  /// <param name="streamType">The type of the stream in which the event resides.</param>
  /// <param name="version">The version of the event.</param>
  /// <param name="actorId">The identifier of the actor who raised the event.</param>
  /// <param name="occurredOn">The date and time when the event occurred.</param>
  /// <param name="isDeleted">A value indicating whether or not the event deletes, undeletes or leaves unchanged its stream deletion status.</param>
  public EventMetadata(Type eventType, EventId? eventId = null, Type? streamType = null, long? version = null, ActorId? actorId = null, DateTime? occurredOn = null, bool? isDeleted = null)
  {
    EventType = eventType;
    EventId = eventId;
    StreamType = streamType;
    Version = version;
    ActorId = actorId;
    OccurredOn = occurredOn;
    IsDeleted = isDeleted;
  }
}
