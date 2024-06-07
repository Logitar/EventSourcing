namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when a past event is applied to an aggregate of a future state.
/// </summary>
public class CannotApplyPastEventException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The specified event is past the current state of the specified aggregate.";

  /// <summary>
  /// Gets or sets the string representation of the aggregate.
  /// </summary>
  public string Aggregate
  {
    get => (string)Data[nameof(Aggregate)]!;
    private set => Data[nameof(Aggregate)] = value;
  }
  /// <summary>
  /// Gets or sets the string representation of the aggregate identifier.
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
  /// Gets or sets the string representation of the event.
  /// </summary>
  public string Event
  {
    get => (string)Data[nameof(Event)]!;
    private set => Data[nameof(Event)] = value;
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
  public long? EventVersion
  {
    get => (long?)Data[nameof(EventVersion)];
    private set => Data[nameof(EventVersion)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="CannotApplyPastEventException"/> class.
  /// </summary>
  /// <param name="aggregate">The aggregate in a future state.</param>
  /// <param name="change">The event of a past state.</param>
  public CannotApplyPastEventException(AggregateRoot aggregate, DomainEvent change) : base(BuildMessage(aggregate, change))
  {
    Aggregate = aggregate.ToString();
    AggregateId = aggregate.Id.ToString();
    AggregateVersion = aggregate.Version;
    Event = change.ToString();
    EventId = change.Id.ToString();
    EventVersion = change.Version;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="aggregate">The aggregate in a future state.</param>
  /// <param name="change">The event of a past state.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(AggregateRoot aggregate, DomainEvent change) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Aggregate), aggregate)
    .AddData(nameof(AggregateId), aggregate.Id)
    .AddData(nameof(AggregateVersion), aggregate.Version)
    .AddData(nameof(Event), change)
    .AddData(nameof(EventId), change.Id)
    .AddData(nameof(EventVersion), change.Version)
    .Build();
}
