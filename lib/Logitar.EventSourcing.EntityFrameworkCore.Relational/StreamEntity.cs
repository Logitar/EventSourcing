namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents the database entity for event streams.
/// </summary>
public class StreamEntity
{
  /// <summary>
  /// Gets or sets the primary identifier (clustered key) of the stream.
  /// </summary>
  public long StreamId { get; private set; }
  /// <summary>
  /// Gets or sets the identifier of the stream.
  /// </summary>
  public string Id { get; private set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of the stream.
  /// </summary>
  public string? Type { get; private set; }

  /// <summary>
  /// Gets or sets the version of the stream.
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who created the stream.
  /// </summary>
  public string? CreatedBy { get; private set; }
  /// <summary>
  /// Gets or sets the date and time when the stream was created.
  /// </summary>
  public DateTime CreatedOn { get; private set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who updated the stream lastly.
  /// </summary>
  public string? UpdatedBy { get; private set; }
  /// <summary>
  /// Gets or sets the date and time when the stream was updated lastly.
  /// </summary>
  public DateTime UpdatedOn { get; private set; }

  /// <summary>
  /// Gets or sets a value indicating whether or not the stream is deleted.
  /// </summary>
  public bool IsDeleted { get; private set; }

  /// <summary>
  /// Gets or sets the event in the stream.
  /// </summary>
  public List<EventEntity> Events { get; private set; } = [];

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamEntity"/> class.
  /// </summary>
  /// <param name="id">The stream identifier.</param>
  /// <param name="type">The stream type.</param>
  public StreamEntity(StreamId id, Type? type = null)
  {
    Id = id.Value;

    Type = type?.GetNamespaceQualifiedName();
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamEntity"/> class.
  /// </summary>
  private StreamEntity()
  {
  }

  /// <summary>
  /// Appends the specified event to the stream.
  /// </summary>
  /// <param name="event">The event to append.</param>
  /// <exception cref="StreamMismatchException">The event does not belong to the current aggregate.</exception>
  /// <exception cref="UnexpectedEventVersionException">The event version is not subsequent to the aggregate version.</exception>
  public void Append(EventEntity @event)
  {
    if (@event.Stream == null || !@event.Stream.Equals(this) || @event.StreamId != StreamId)
    {
      throw new StreamMismatchException(this, @event);
    }
    if (@event.Version != (Version + 1))
    {
      throw new UnexpectedEventVersionException(this, @event);
    }

    Version++;

    if (Version <= 1)
    {
      CreatedBy = @event.ActorId;
      CreatedOn = @event.OccurredOn;
    }

    UpdatedBy = @event.ActorId;
    UpdatedOn = @event.OccurredOn;

    if (@event.IsDeleted.HasValue)
    {
      IsDeleted = @event.IsDeleted.Value;
    }

    Events.Add(@event);
  }

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the stream.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the stream.</returns>
  public override bool Equals(object? obj) => obj is StreamEntity stream && stream.Id == Id;
  /// <summary>
  /// Returns the hash code of the current stream.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => Id.GetHashCode();
  /// <summary>
  /// Returns a string representation of the stream.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => Type == null ? $"{GetType()} (Id={Id})" : $"{Type} | {GetType()} (Id={Id})";
}
