namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// The exception thrown when a past event is applied to an stream of a future state.
/// </summary>
public class UnexpectedEventVersionException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The version of the specified event was not expected.";

  /// <summary>
  /// Gets or sets the identifier of the stream.
  /// </summary>
  public string StreamId
  {
    get => (string)Data[nameof(StreamId)]!;
    private set => Data[nameof(StreamId)] = value;
  }
  /// <summary>
  /// Gets or sets the version of the stream.
  /// </summary>
  public long StreamVersion
  {
    get => (long)Data[nameof(StreamVersion)]!;
    private set => Data[nameof(StreamVersion)] = value;
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
  /// Gets or sets the version of the event.
  /// </summary>
  public long EventVersion
  {
    get => (long)Data[nameof(EventVersion)]!;
    private set => Data[nameof(EventVersion)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="UnexpectedEventVersionException"/> class.
  /// </summary>
  /// <param name="stream">The stream.</param>
  /// <param name="event">The unexpected event.</param>
  public UnexpectedEventVersionException(StreamEntity stream, EventEntity @event) : base(BuildMessage(stream, @event))
  {
    StreamId = stream.Id;
    StreamVersion = stream.Version;
    EventId = @event.Id;
    EventVersion = @event.Version;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="stream">The stream.</param>
  /// <param name="event">The unexpected event.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(StreamEntity stream, EventEntity @event) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(StreamId), stream.Id)
    .AddData(nameof(StreamVersion), stream.Version)
    .AddData(nameof(EventId), @event.Id)
    .AddData(nameof(EventVersion), @event.Version)
    .Build();
}
