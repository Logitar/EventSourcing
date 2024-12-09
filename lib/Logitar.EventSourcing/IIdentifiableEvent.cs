namespace Logitar.EventSourcing;

/// <summary>
/// TODO(fpion): document
/// </summary>
public interface IIdentifiableEvent : IEvent
{
  /// <summary>
  /// Gets the identifier of the event.
  /// </summary>
  EventId Id { get; }
}
