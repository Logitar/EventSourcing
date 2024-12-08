namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Implements a serializer for events.
/// </summary>
public class EventSerializer : IEventSerializer
{
  /// <summary>
  /// Gets or sets the serializer options.
  /// </summary>
  protected JsonSerializerOptions SerializerOptions { get; set; }

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
  /// Serializes the specified event.
  /// </summary>
  /// <param name="event">The event to serialize.</param>
  /// <returns>The serialized string representation of the event.</returns>
  public virtual string Serialize(object @event) => JsonSerializer.Serialize(@event, @event.GetType(), SerializerOptions);

  /// <summary>
  /// Registers the default and specified JSON converters.
  /// </summary>
  /// <param name="converters">The JSON converters.</param>
  protected virtual void RegisterConverters(params JsonConverter[] converters)
  {
    SerializerOptions.Converters.Add(new ActorIdConverter());
    SerializerOptions.Converters.Add(new EventIdConverter());
    SerializerOptions.Converters.Add(new StreamIdConverter());

    foreach (JsonConverter converter in converters)
    {
      SerializerOptions.Converters.Add(converter);
    }
  }
}
