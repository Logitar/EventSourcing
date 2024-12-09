namespace Logitar.EventSourcing;

public interface IVersionedEvent : IEvent
{
  long Version { get; }
}
