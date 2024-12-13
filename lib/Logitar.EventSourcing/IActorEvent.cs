namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event that has been raised by an actor.
/// </summary>
public interface IActorEvent : IEvent
{
  /// <summary>
  /// Gets the identifier of the actor who raised the event.
  /// </summary>
  ActorId? ActorId { get; }
}
