namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a JSON serializer for events.
/// </summary>
public class EventSerializer : IEventSerializer
{
  /// <summary>
  /// The serialization options.
  /// </summary>
  private readonly JsonSerializerOptions _options = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="EventSerializer"/> class.
  /// </summary>
  public EventSerializer()
  {
    RegisterConverter(new ActorIdConverter());
    RegisterConverter(new AggregateIdConverter());
    RegisterConverter(new JsonStringEnumConverter());
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="EventSerializer"/> class.
  /// </summary>
  /// <param name="converters">The JSON converters to register to the serializer.</param>
  public EventSerializer(IEnumerable<JsonConverter> converters) : this()
  {
    foreach (JsonConverter converter in converters)
    {
      RegisterConverter(converter);
    }
  }

  /// <summary>
  /// Registers the specified JSON converter to the serializer.
  /// </summary>
  /// <param name="converter">The converter to register.</param>
  public void RegisterConverter(JsonConverter converter)
  {
    _options.Converters.Add(converter);
  }

  /// <summary>
  /// Deserializes an event from its storage model.
  /// </summary>
  /// <param name="entity">The storage model from which to deserialize.</param>
  /// <returns>The deserialized event.</returns>
  /// <exception cref="EventTypeNotFoundException">The event type could not be found.</exception>
  /// <exception cref="EventDataDeserializationFailedException">The event data deserialization failed.</exception>
  public DomainEvent Deserialize(IEventEntity entity)
  {
    Type eventType = Type.GetType(entity.EventType) ?? throw new EventTypeNotFoundException(entity);

    return (DomainEvent?)JsonSerializer.Deserialize(entity.EventData, eventType, _options)
      ?? throw new EventDataDeserializationFailedException(entity);
  }
  /// <summary>
  /// Serializes the specified event to JSON.
  /// </summary>
  /// <param name="change">The event to serialize.</param>
  /// <returns>The resulting JSON.</returns>
  public string Serialize(DomainEvent change)
  {
    return JsonSerializer.Serialize(change, change.GetType(), _options);
  }
}
