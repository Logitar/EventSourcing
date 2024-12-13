namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when an event is applied to the wrong aggregate.
/// </summary>
public class StreamMismatchException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The stream identifier of the specified event did not match the identifier of the specified stream.";

  /// <summary>
  /// Gets or sets the identifier of the aggregate.
  /// </summary>
  public string AggregateStreamId
  {
    get => (string)Data[nameof(AggregateStreamId)]!;
    private set => Data[nameof(AggregateStreamId)] = value;
  }
  /// <summary>
  /// Gets or sets the stream identifier of the event.
  /// </summary>
  public string EventStreamId
  {
    get => (string)Data[nameof(EventStreamId)]!;
    private set => Data[nameof(EventStreamId)] = value;
  }
  /// <summary>
  /// Gets or sets the identifier of the event.
  /// </summary>
  public string? EventId
  {
    get => (string)Data[nameof(EventId)];
    private set => Data[nameof(EventId)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamMismatchException"/> class.
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied.</param>
  /// <param name="event">The event belonging to another aggregate.</param>
  public StreamMismatchException(IAggregate aggregate, IStreamEvent @event) : base(BuildMessage(aggregate, @event))
  {
    AggregateStreamId = aggregate.Id.Value;
    EventStreamId = @event.StreamId.Value;

    if (@event is IIdentifiableEvent identifiable)
    {
      EventId = identifiable.Id.Value;
    }
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied.</param>
  /// <param name="event">The event belonging to another aggregate.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(IAggregate aggregate, IStreamEvent @event) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(AggregateStreamId), aggregate.Id)
    .AddData(nameof(EventStreamId), @event.StreamId)
    .AddData(nameof(EventId), @event is IIdentifiableEvent identifiable ? identifiable.Id : null, "<null>")
    .Build();
}
