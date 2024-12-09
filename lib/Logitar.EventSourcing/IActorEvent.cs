namespace Logitar.EventSourcing;

/// <summary>
/// TODO(fpion): document
/// </summary>
public interface IActorEvent : IEvent
{
  /// <summary>
  /// TODO(fpion): document
  /// </summary>
  ActorId? ActorId { get; }
}
