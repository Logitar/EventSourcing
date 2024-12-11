namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// The exception thrown when an event is applied to the wrong stream.
/// </summary>
public class StreamMismatchException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The stream identifier of the specified event did not match the identifier of the specified stream.";

  /// <summary>
  /// Gets or sets the identifier of the stream.
  /// </summary>
  public string StreamId
  {
    get => (string)Data[nameof(StreamId)]!;
    private set => Data[nameof(StreamId)] = value;
  }
  /// <summary>
  /// Gets or sets the stream identifier of the event.
  /// </summary>
  public string? EventStreamId
  {
    get => (string?)Data[nameof(EventStreamId)];
    private set => Data[nameof(EventStreamId)] = value;
  }
  /// <summary>
  /// Gets or sets the identifier of the event.
  /// </summary>
  public string EventId
  {
    get => (string)Data[nameof(EventId)]!;
    private set => Data[nameof(EventId)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamMismatchException"/> class.
  /// </summary>
  /// <param name="stream">The stream unto which the event was applied.</param>
  /// <param name="event">The event belonging to another stream.</param>
  public StreamMismatchException(StreamEntity stream, EventEntity @event) : base(BuildMessage(stream, @event))
  {
    StreamId = stream.Id;
    EventStreamId = @event.Stream?.Id;
    EventId = @event.Id;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="stream">The stream unto which the event was applied.</param>
  /// <param name="event">The event belonging to another stream.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(StreamEntity stream, EventEntity @event) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(StreamId), stream.Id)
    .AddData(nameof(EventStreamId), @event.Stream?.Id, "<null>")
    .AddData(nameof(EventId), @event.Id)
    .Build();
}
