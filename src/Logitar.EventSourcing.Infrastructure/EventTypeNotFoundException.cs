namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// The exception thrown when the type of an event could not be found.
/// </summary>
public class EventTypeNotFoundException : Exception
{
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
  /// Gets or sets the identifier of the invalid event.
  /// </summary>
  public Guid EventId
  {
    get => (Guid)Data[nameof(EventId)]!;
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
  /// Builds the exception message.
  /// </summary>
  /// <param name="entity">The invalid event.</param>
  /// <returns>The exception message.</returns>
  private static string BuildMessage(IEventEntity entity)
  {
    StringBuilder message = new();

    message.AppendLine("The specified event type could not be found.");
    message.Append("EventId: ").Append(entity.Id).AppendLine();
    message.Append("TypeName: ").AppendLine(entity.EventType);

    return message.ToString();
  }
}
