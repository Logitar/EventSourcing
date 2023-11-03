namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// The exception thrown when the deserialization of an event's data failed or returned null.
/// </summary>
public class EventDataDeserializationFailedException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventDataDeserializationFailedException"/> class.
  /// </summary>
  /// <param name="entity">The invalid event.</param>
  internal EventDataDeserializationFailedException(IEventEntity entity) : base(BuildMessage(entity))
  {
    EventId = entity.Id;
    EventType = entity.EventType;
    EventData = entity.EventData;
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
  /// Gets or sets the type of the invalid event.
  /// </summary>
  public string EventType
  {
    get => (string)Data[nameof(EventType)]!;
    private set => Data[nameof(EventType)] = value;
  }
  /// <summary>
  /// Gets or sets the data of the invalid event.
  /// </summary>
  public string EventData
  {
    get => (string)Data[nameof(EventData)]!;
    private set => Data[nameof(EventData)] = value;
  }

  /// <summary>
  /// Builds the exception message.
  /// </summary>
  /// <param name="entity">The invalid event.</param>
  /// <returns>The exception message</returns>
  private static string BuildMessage(IEventEntity entity)
  {
    StringBuilder message = new();

    message.AppendLine("The specified event data could not be deserialized.");
    message.Append("EventId: ").Append(entity.Id).AppendLine();
    message.Append("EventType: ").AppendLine(entity.EventType);
    message.Append("EventData: ").AppendLine(entity.EventData);

    return message.ToString();
  }
}
