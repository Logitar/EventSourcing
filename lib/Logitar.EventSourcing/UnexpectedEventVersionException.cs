namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when a past event is applied to an aggregate of a future state.
/// </summary>
public class UnexpectedEventVersionException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The version of the specified version was not expected.";

  /// <summary>
  /// Gets or sets the identifier of the aggregate.
  /// </summary>
  public string AggregateId
  {
    get => (string)Data[nameof(AggregateId)]!;
    private set => Data[nameof(AggregateId)] = value;
  }
  /// <summary>
  /// Gets or sets the version of the aggregate.
  /// </summary>
  public long AggregateVersion
  {
    get => (long)Data[nameof(AggregateVersion)]!;
    private set => Data[nameof(AggregateVersion)] = value;
  }
  /// <summary>
  /// Gets or sets the identifier of the event.
  /// </summary>
  public string? EventId
  {
    get => (string?)Data[nameof(EventId)];
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
  /// <param name="aggregate">The aggregate.</param>
  /// <param name="event">The unexpected event.</param>
  public UnexpectedEventVersionException(IVersionedAggregate aggregate, IVersionedEvent @event) : base(BuildMessage(aggregate, @event))
  {
    AggregateId = aggregate.Id.Value;
    AggregateVersion = aggregate.Version;
    EventId = @event is IIdentifiableEvent identifiable ? identifiable.Id.Value : null;
    EventVersion = @event.Version;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="aggregate">The aggregate.</param>
  /// <param name="event">The unexpected event.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(IVersionedAggregate aggregate, IVersionedEvent @event) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(AggregateId), aggregate.Id)
    .AddData(nameof(AggregateVersion), aggregate.Version)
    .AddData(nameof(EventId), @event is IIdentifiableEvent identifiable ? identifiable.Id : null, "<null>")
    .AddData(nameof(EventVersion), @event.Version)
    .Build();
}
