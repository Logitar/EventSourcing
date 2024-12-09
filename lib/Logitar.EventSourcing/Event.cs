namespace Logitar.EventSourcing;

/// <summary>
/// TODO(fpion): document
/// </summary>
public class Event // TODO(fpion): unit tests
{
  /// <summary>
  /// Gets the identifier of the event.
  /// </summary>
  public EventId Id { get; } // TODO(fpion): nullable?
  /// <summary>
  /// Gets the type of the event.
  /// </summary>
  public Type? Type { get; } // TODO(fpion): nullable?
  /// <summary>
  /// Gets the version of the event.
  /// </summary>
  public long? Version { get; } // TODO(fpion): nullable?

  /// <summary>
  /// Gets the identifier of the actor who raised the event.
  /// </summary>
  public ActorId? ActorId { get; }
  /// <summary>
  /// Gets the date and time when the event was raised.
  /// </summary>
  public DateTime? OccurredOn { get; } // TODO(fpion): nullable?
  /// <summary>
  /// Gets a value indicating whether or not the stream is deleted, undeleted or unchanged.
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
  /// <param name="data">The event data.</param>
  /// <param name="type">The type of the event.</param>
  /// <param name="version">The version of the event.</param>
  /// <param name="actorId">The identifier of the actor who raised the event.</param>
  /// <param name="occurredOn">The date and time when the event was raised.</param>
  /// <param name="isDeleted">A value indicating whether or not the stream is deleted, undeleted or unchanged.</param>
  public Event(EventId id, IEvent data, Type? type = null, long? version = null, ActorId? actorId = null, DateTime? occurredOn = null, bool? isDeleted = null)
  {
    Id = id;
    Type = type;
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
  public override string ToString() => Type == null ? Id.Value : $"{Type} (Id={Id})";
}
