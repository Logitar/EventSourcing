namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when an event is applied to the wrong aggregate.
/// </summary>
public class EventAggregateMismatchException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The specified event does not belong to the specified aggregate.";

  /// <summary>
  /// Gets or sets the type of the aggregate.
  /// </summary>
  public string AggregateType
  {
    get => (string)Data[nameof(AggregateType)]!;
    private set => Data[nameof(AggregateType)] = value;
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
  /// Gets or sets the type of the event.
  /// </summary>
  public string EventType
  {
    get => (string)Data[nameof(EventType)]!;
    private set => Data[nameof(EventType)] = value;
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
  /// Gets or sets the string representation of the event aggregate identifier.
  /// </summary>
  public string EventAggregateId
  {
    get => (string)Data[nameof(EventAggregateId)]!;
    private set => Data[nameof(EventAggregateId)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventAggregateMismatchException"/> class.
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied.</param>
  /// <param name="change">The event belonging to another aggregate.</param>
  public EventAggregateMismatchException(AggregateRoot aggregate, DomainEvent change) : base(BuildMessage(aggregate, change))
  {
    AggregateType = aggregate.GetType().GetNamespaceQualifiedName();
    AggregateId = aggregate.Id.ToString();
    EventType = change.GetType().GetNamespaceQualifiedName();
    EventId = change.Id.ToString();
    EventAggregateId = change.AggregateId.ToString();
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied.</param>
  /// <param name="change">The event belonging to another aggregate.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(AggregateRoot aggregate, DomainEvent change) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(AggregateType), aggregate.GetType().GetNamespaceQualifiedName())
    .AddData(nameof(AggregateId), aggregate.Id)
    .AddData(nameof(EventType), change.GetType().GetNamespaceQualifiedName())
    .AddData(nameof(EventId), change.Id)
    .AddData(nameof(EventAggregateId), change.AggregateId)
    .Build();
}

