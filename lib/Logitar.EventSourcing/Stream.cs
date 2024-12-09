using System.Linq;

namespace Logitar.EventSourcing;

/// <summary>
/// Represents a stream of events.
/// </summary>
public class Stream // TODO(fpion): unit tests
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
  /// Gets the identifier of the actor who created the stream.
  /// </summary>
  public ActorId? CreatedBy { get; }
  /// <summary>
  /// Gets the date and time when the stream was created.
  /// </summary>
  public DateTime? CreatedOn { get; }

  /// <summary>
  /// Gets the identifier of the actor who updated the stream lastly.
  /// </summary>
  public ActorId? UpdatedBy { get; }
  /// <summary>
  /// Gets the date and time when the stream was updated lastly.
  /// </summary>
  public DateTime? UpdatedOn { get; }

  /// <summary>
  /// Gets a value indicating whether or not the stream is deleted.
  /// </summary>
  public bool IsDeleted { get; }

  /// <summary>
  /// Gets the events in the stream.
  /// </summary>
  public IReadOnlyCollection<Event> Events { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Stream"/> class.
  /// </summary>
  /// <param name="id">The stream identifier.</param>
  /// <param name="type">The stream type.</param>
  /// <param name="version">The stream version.</param>
  /// <param name="createdBy">The identifier of the actor who created the stream.</param>
  /// <param name="createdOn">The date and time when the stream was created.</param>
  /// <param name="updatedBy">The identifier of the actor who updated the stream lastly.</param>
  /// <param name="updatedOn">The date and time when the stream was updated lastly.</param>
  /// <param name="isDeleted">A value indicating whether or not the stream is deleted.</param>
  /// <param name="events">The events in the stream.</param>
  public Stream(StreamId id, Type? type = null, long version = 0, ActorId? createdBy = null, DateTime? createdOn = null,
    ActorId? updatedBy = null, DateTime? updatedOn = null, bool isDeleted = false, IEnumerable<Event>? events = null)
  {
    Id = id;
    Type = type;
    Version = version;

    CreatedBy = createdBy;
    CreatedOn = createdOn;

    UpdatedBy = updatedBy;
    UpdatedOn = updatedOn;

    IsDeleted = isDeleted;

    Events = events?.ToArray() ?? [];
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
  public override string ToString() => Type == null ? Id.Value : $"{Type} (Id={Id})";
}
