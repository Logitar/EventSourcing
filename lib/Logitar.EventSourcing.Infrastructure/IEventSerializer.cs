namespace Logitar.EventSourcing.Infrastructure;

public interface IEventSerializer
{
  IEvent Deserialize(Type type, string json);
  string Serialize(IEvent @event);
}
