namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents the database entity for events.
/// </summary>
public class EventEntity
{
  /// <summary>
  /// Gets or sets the primary identifier (clustered key) of the event.
  /// </summary>
  public long EventId { get; private set; }
  /// <summary>
  /// Gets or sets the identifier of the event.
  /// </summary>
  public string Id { get; private set; } = string.Empty;

  /// <summary>
  /// Gets or sets the stream in which the event belongs.
  /// </summary>
  public StreamEntity? Stream { get; private set; }
  /// <summary>
  /// Gets or sets the primary identifier (clustered key) of the stream in which the event belongs.
  /// </summary>
  public long StreamId { get; private set; }
  /// <summary>
  /// Gets or sets the version of the event.
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who raised the event.
  /// </summary>
  public string? ActorId { get; private set; }
  /// <summary>
  /// Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime OccurredOn { get; private set; }

  /// <summary>
  /// Gets or sets a value indicating if this event deletes, undeletes or leaves unchanged the stream deletion status.
  /// </summary>
  public bool? IsDeleted { get; private set; }

  /// <summary>
  /// Gets or sets the name of the event type.
  /// </summary>
  public string TypeName { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets the string representation of the namespaced event type.
  /// </summary>
  public string NamespacedType { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets the event data.
  /// </summary>
  public string Data { get; private set; } = string.Empty;

  /// <summary>
  /// Initializes a new instance of the <see cref="EventEntity"/> class.
  /// </summary>
  /// <param name="id">The event identifier.</param>
  /// <param name="stream">The stream in which the event belongs.</param>
  /// <param name="occurredOn">The date and time when the event occurred.</param>
  /// <param name="typeName">The name of the event type.</param>
  /// <param name="namespacedType">The string representation of the namespaced event type.</param>
  /// <param name="data">The event data.</param>
  /// <param name="actorId">The identifier of the actor who raised the event.</param>
  /// <param name="isDeleted">A value indicating if this event deletes, undeletes or leaves unchanged the stream deletion status.</param>
  public EventEntity(EventId id, StreamEntity stream, DateTime occurredOn, string typeName, string namespacedType, string data, ActorId? actorId = null, bool? isDeleted = null)
  {
    Id = id.Value;

    Stream = stream;
    StreamId = stream.StreamId;
    Version = stream.Version + 1;

    ActorId = actorId?.Value;
    OccurredOn = occurredOn.AsUniversalTime();

    IsDeleted = isDeleted;

    TypeName = typeName;
    NamespacedType = namespacedType;
    Data = data;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventEntity"/> class.
  /// </summary>
  private EventEntity()
  {
  }

  /// <summary>
  /// Returns the type of the event data.
  /// </summary>
  /// <returns>The event data type.</returns>
  public Type GetDataType() => Type.GetType(NamespacedType) ?? throw new EventTypeNotFoundException(this);

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the event.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the event.</returns>
  public override bool Equals(object? obj) => obj is EventEntity @event && @event.Id == Id;
  /// <summary>
  /// Returns the hash code of the current event.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => Id.GetHashCode();
  /// <summary>
  /// Returns a string representation of the event.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{TypeName} | {GetType()} (Id={Id})";
}
