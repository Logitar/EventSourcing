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
  /// Gets or sets the version of the aggregate.
  /// </summary>
  public long AggregateVersion
  {
    get => (long)Data[nameof(AggregateVersion)]!;
    private set => Data[nameof(AggregateVersion)] = value;
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
  /// Gets or sets the version of the event.
  /// </summary>
  public long EventVersion
  {
    get => (long)Data[nameof(EventVersion)]!;
    private set => Data[nameof(EventVersion)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="CannotApplyPastEventException"/> class.
  /// </summary>
  /// <param name="aggregate">The aggregate in a future state.</param>
  /// <param name="change">The event of a past state.</param>
  public CannotApplyPastEventException(AggregateRoot aggregate, DomainEvent change) : base(BuildMessage(aggregate, change))
  {
    AggregateType = aggregate.GetType().GetNamespaceQualifiedName();
    AggregateId = aggregate.Id.ToString();
    AggregateVersion = aggregate.Version;
    EventType = change.GetType().GetNamespaceQualifiedName();
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
    .AddData(nameof(AggregateType), aggregate.GetType().GetNamespaceQualifiedName())
    .AddData(nameof(AggregateId), aggregate.Id)
    .AddData(nameof(AggregateVersion), aggregate.Version)
    .AddData(nameof(EventType), change.GetType().GetNamespaceQualifiedName())
    .AddData(nameof(EventId), change.Id)
    .AddData(nameof(EventVersion), change.Version)
    .Build();
}
