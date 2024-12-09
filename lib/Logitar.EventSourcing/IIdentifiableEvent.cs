namespace Logitar.EventSourcing;

public interface IIdentifiableEvent : IEvent
{
  EventId Id { get; }
}
