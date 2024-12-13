namespace Logitar.EventSourcing;

/// <summary>
/// Represents a stream of events.
/// </summary>
public class Stream
{
  /// <summary>
  /// Gets the identifier of the stream.
  /// </summary>
  public StreamId Id { get; }
  /// <summary>
  /// Gets the type of the stream.
  /// </summary>
  public Type? Type { get; }
  /// <summary>
  /// Gets the version of the stream.
  /// </summary>
  public long Version { get; }

  /// <summary>
  /// Gets or sets the identifier of the actor who created the stream.
  /// </summary>
  public ActorId? CreatedBy { get; private set; }
  /// <summary>
  /// Gets or sets the date and time when the stream was created.
  /// </summary>
  public DateTime? CreatedOn { get; private set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who updated the stream lastly.
  /// </summary>
  public ActorId? UpdatedBy { get; private set; }
  /// <summary>
  /// Gets or sets the date and time when the stream was updated lastly.
  /// </summary>
  public DateTime? UpdatedOn { get; private set; }

  /// <summary>
  /// Gets or sets a value indicating whether or not the stream is deleted.
  /// </summary>
  public bool IsDeleted { get; private set; }

  /// <summary>
  /// Gets or sets the events in the stream.
  /// </summary>
  public IReadOnlyCollection<Event> Events { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Stream"/> class.
  /// </summary>
  /// <param name="id">The identifier of the stream.</param>
  /// <param name="type">The type of the stream.</param>
  /// <param name="events">The events in the stream.</param>
  public Stream(StreamId id, Type? type = null, IEnumerable<Event>? events = null)
  {
    Id = id;
    Type = type;

    if (events == null)
    {
      Events = [];
    }
    else
    {
      Events = events.ToArray();

      foreach (Event @event in events)
      {
        Version = @event.Version;

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
      }
    }
  }

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the stream.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the stream.</returns>
  public override bool Equals(object obj) => obj is Stream stream && stream.Id == Id;
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
