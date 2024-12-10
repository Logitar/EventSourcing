namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event belonging to a stream and that has been applied to it.
/// </summary>
public class Event : IActorEvent, IDeleteControlEvent, IIdentifiableEvent, ITemporalEvent, IVersionedEvent
{
  /// <summary>
  /// Gets the identifier of the event.
  /// </summary>
  public EventId Id { get; }

  /// <summary>
  /// Gets the version of the event.
  /// </summary>
  public long Version { get; }

  /// <summary>
  /// Gets the identifier of the actor who raised the event.
  /// </summary>
  public ActorId? ActorId { get; }
  /// <summary>
  /// Gets the date and time when the event occurred.
  /// </summary>
  public DateTime OccurredOn { get; }

  /// <summary>
  /// Gets a value indicating whether or not the stream is deleted.
  /// </summary>
  public bool? IsDeleted { get; }

  /// <summary>
  /// Gets the event data.
  /// </summary>
  public IEvent Data { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Event"/> class.
  /// </summary>
  /// <param name="id">The identifier of the event.</param>
  /// <param name="version">The version of the event.</param>
  /// <param name="occurredOn">The date and time when the event occurred.</param>
  /// <param name="data">The event data.</param>
  /// <param name="actorId">The identifier of the actor who raised the event.</param>
  /// <param name="isDeleted">A value indicating whether or not the stream is deleted.</param>
  public Event(EventId id, long version, DateTime occurredOn, IEvent data, ActorId? actorId = null, bool? isDeleted = null)
  {
    Id = id;

    Version = version;

    ActorId = actorId;
    OccurredOn = occurredOn;

    IsDeleted = isDeleted;

    Data = data;
  }

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the event.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the event.</returns>
  public override bool Equals(object obj) => obj is Event @event && @event.Id == Id;
  /// <summary>
  /// Returns the hash code of the current event.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => Id.GetHashCode();
  /// <summary>
  /// Returns a string representation of the event.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{Data.GetType()} | {GetType()} (Id={Id})";
}
