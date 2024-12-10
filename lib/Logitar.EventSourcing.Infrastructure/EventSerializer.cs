namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a JSON serializer for events.
/// </summary>
public class EventSerializer : IEventSerializer
{
  /// <summary>
  /// Gets the serializer options.
  /// </summary>
  protected JsonSerializerOptions SerializerOptions { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventSerializer"/> class.
  /// </summary>
  public EventSerializer()
  {
    SerializerOptions = new JsonSerializerOptions();
    RegisterConverters();
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventSerializer"/> class.
  /// </summary>
  /// <param name="serializerOptions">The serializer options.</param>
  public EventSerializer(JsonSerializerOptions serializerOptions)
  {
    SerializerOptions = serializerOptions;
    RegisterConverters();
  }

  /// <summary>
  /// Deserializes the specified event.
  /// </summary>
  /// <param name="type">The type of the event.</param>
  /// <param name="json">The JSON representation of the event.</param>
  /// <returns>The deserialized event.</returns>
  /// <exception cref="EventDeserializationFailedException">The deserialization of the event failed, returning null.</exception>
  public virtual IEvent Deserialize(Type type, string json)
  {
    return JsonSerializer.Deserialize(json, type, SerializerOptions) as IEvent
      ?? throw new EventDeserializationFailedException(type, json);
  }

  /// <summary>
  /// Serializes the specified event.
  /// </summary>
  /// <param name="event">The event to serialize.</param>
  /// <returns>The resulting JSON.</returns>
  public virtual string Serialize(IEvent @event) => JsonSerializer.Serialize(@event, @event.GetType(), SerializerOptions);

  /// <summary>
  /// Registers JSON converters into the serializer options.
  /// </summary>
  protected virtual void RegisterConverters()
  {
    SerializerOptions.Converters.Add(new ActorIdConverter());
    SerializerOptions.Converters.Add(new EventIdConverter());
    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    SerializerOptions.Converters.Add(new StreamIdConverter());
  }
}
