namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents an operation that apprends events to a stream.
/// </summary>
public sealed record AppendToStream
{
  /// <summary>
  /// Gets the stream identifier.
  /// </summary>
  public StreamId Id { get; }

  /// <summary>
  /// Gets the stream type.
  /// </summary>
  public Type? Type { get; }

  /// <summary>
  /// Gets the stream expectation.
  /// </summary>
  public StreamExpectation Expectation { get; }

  /// <summary>
  /// Gets the events to append.
  /// </summary>
  public IEnumerable<object> Events { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="AppendToStream"/> class.
  /// </summary>
  /// <param name="id">The stream identifier.</param>
  /// <param name="type">The stream type.</param>
  /// <param name="expectation">The stream expectation.</param>
  /// <param name="events">The events to append.</param>
  public AppendToStream(StreamId id, Type? type, StreamExpectation expectation, IEnumerable<object> events)
  {
    Id = id;
    Type = type;
    Expectation = expectation;
    Events = events;
  }
}
