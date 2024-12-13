namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain event belonging to a stream and that can be applied to it.
/// </summary>
public abstract record DomainEvent : IActorEvent, IDeleteControlEvent, IIdentifiableEvent, IStreamEvent, ITemporalEvent, IVersionedEvent
{
  /// <summary>
  /// Gets or sets the identifier of the event.
  /// </summary>
  public EventId Id { get; set; } = EventId.NewId();

  /// <summary>
  /// Gets or sets the identifier of the stream to which the event belongs to.
  /// </summary>
  public StreamId StreamId { get; set; }
  /// <summary>
  /// Gets or sets the version of the event.
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who raised the event.
  /// </summary>
  public ActorId? ActorId { get; set; }
  /// <summary>
  /// Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime OccurredOn { get; set; } = DateTime.Now;

  /// <summary>
  /// Gets or sets a value indicating whether or not the stream is deleted.
  /// </summary>
  public bool? IsDeleted { get; set; }
}
