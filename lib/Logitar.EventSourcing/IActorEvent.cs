namespace Logitar.EventSourcing;

public interface IActorEvent : IEvent
{
  ActorId? ActorId { get; }
}
