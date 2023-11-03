namespace Logitar.EventSourcing;

/// <summary>
/// The exception thrown when a past event is applied to an aggregate of a future state.
/// </summary>
public class CannotApplyPastEventException : Exception
{
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
    EventId = change.Id;
    EventVersion = change.Version;
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
  public Guid? EventId
  {
    get => (Guid?)Data[nameof(EventId)];
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
  /// Builds the exception message.
  /// </summary>
  /// <param name="aggregate">The aggregate in a future state.</param>
  /// <param name="change">The event of a past state.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(AggregateRoot aggregate, DomainEvent change)
  {
    StringBuilder message = new();

    message.AppendLine("The specified event is past the current state of the specified aggregate.");
    message.Append("Aggregate: ").Append(aggregate).AppendLine();
    message.Append("AggregateId: ").Append(aggregate.Id).AppendLine();
    message.Append("AggregateVersion: ").Append(aggregate.Version).AppendLine();
    message.Append("Event: ").Append(change).AppendLine();
    message.Append("EventId: ").Append(change.Id).AppendLine();
    message.Append("EventVersion: ").Append(change.Version).AppendLine();

    return message.ToString();
  }
}
