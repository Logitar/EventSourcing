
namespace Logitar.EventSourcing.Infrastructure;

public class EventSerializer : IEventSerializer
{
  protected JsonSerializerOptions SerializerOptions { get; }

  public EventSerializer()
  {
    SerializerOptions = new JsonSerializerOptions();
    RegisterConverters();
  }

  public EventSerializer(JsonSerializerOptions serializerOptions)
  {
    SerializerOptions = serializerOptions;
    RegisterConverters();
  }

  public virtual IEvent Deserialize(Type type, string json)
  {
    return JsonSerializer.Deserialize(json, type, SerializerOptions) as IEvent
      ?? throw new NotImplementedException();
  }

  public virtual string Serialize(IEvent @event) => JsonSerializer.Serialize(@event, SerializerOptions);

  protected virtual void RegisterConverters()
  {
    SerializerOptions.Converters.Add(new ActorIdConverter());
    SerializerOptions.Converters.Add(new EventIdConverter());
    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    SerializerOptions.Converters.Add(new StreamIdConverter());
  }
}
