namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

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
  /// <param name="event">The event.</param>
  public EventTypeNotFoundException(EventEntity @event) : base(BuildMessage(@event))
  {
    EventId = @event.Id;
    TypeName = @event.NamespacedType;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="event">The invalid event.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(EventEntity @event) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(EventId), @event.Id)
    .AddData(nameof(TypeName), @event.NamespacedType)
    .Build();
}
