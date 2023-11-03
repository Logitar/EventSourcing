namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when an event is applied to the wrong aggregate.
/// </summary>
public class EventAggregateMismatchException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventAggregateMismatchException"/> class.
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied.</param>
  /// <param name="change">The event belonging to another aggregate.</param>
  public EventAggregateMismatchException(AggregateRoot aggregate, DomainEvent change) : base(BuildMessage(aggregate, change))
  {
    Aggregate = aggregate.ToString();
    AggregateId = aggregate.Id.ToString();
    Event = change.ToString();
    EventId = change.Id;
    EventAggregateId = change.AggregateId.ToString();
  }

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
  public Guid EventId
  {
    get => (Guid)Data[nameof(EventId)]!;
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
  /// Builds the exception message.
  /// </summary>
  /// <param name="aggregate">The aggregate unto which the event was applied.</param>
  /// <param name="change">The event belonging to another aggregate.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(AggregateRoot aggregate, DomainEvent change)
  {
    StringBuilder message = new();

    message.AppendLine("The specified event does not belong to the specified aggregate.");
    message.Append("Aggregate: ").Append(aggregate).AppendLine();
    message.Append("AggregateId: ").Append(aggregate.Id).AppendLine();
    message.Append("Event: ").Append(change).AppendLine();
    message.Append("EventId: ").Append(change.Id).AppendLine();
    message.Append("EventAggregateId: ").Append(change.AggregateId).AppendLine();

    return message.ToString();
  }
}
