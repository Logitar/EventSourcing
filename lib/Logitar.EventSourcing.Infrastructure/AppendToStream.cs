namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents an operation of appending events to a stream.
/// </summary>
public record AppendToStream
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
  /// Gets an expectation of the stream state.
  /// </summary>
  public StreamExpectation Expectation { get; }
  /// <summary>
  /// Gets the events to append.
  /// </summary>
  public IEnumerable<IEvent> Events { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="AppendToStream"/> class.
  /// </summary>
  /// <param name="id">The identifier of the stream.</param>
  /// <param name="type">The type of the stream.</param>
  /// <param name="expectation">An expectation of the stream state.</param>
  /// <param name="events">The events to append.</param>
  public AppendToStream(StreamId id, Type? type, StreamExpectation expectation, IEnumerable<IEvent> events)
  {
    Id = id;
    Type = type;
    Expectation = expectation;
    Events = events;
  }
}
