namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// The exception thrown when the type of an event could not be found.
/// </summary>
public class EventTypeNotFoundException : Exception
{
  /// <summary>
  /// The detailed error message.
  /// </summary>
  private const string ErrorMessage = "The specified event type could not be found.";

  /// <summary>
  /// Gets or sets the identifier of the invalid event.
  /// </summary>
  public string EventId
  {
    get => (string)Data[nameof(EventId)]!;
    private set => Data[nameof(EventId)] = value;
  }
  /// <summary>
  /// Gets or sets the name of the type that could not be found.
  /// </summary>
  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventTypeNotFoundException"/> class.
  /// </summary>
  /// <param name="entity">The invalid event.</param>
  internal EventTypeNotFoundException(IEventEntity entity) : base(BuildMessage(entity))
  {
    EventId = entity.Id;
    TypeName = entity.EventType;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="entity">The invalid event.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(IEventEntity entity) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(EventId), entity.Id)
    .AddData(nameof(TypeName), entity.EventType)
    .Build();
}
