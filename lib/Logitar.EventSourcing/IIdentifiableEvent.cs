namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event which can be identified by an identifier.
/// </summary>
public interface IIdentifiableEvent : IEvent
{
  /// <summary>
  /// Gets the identifier of the event.
  /// </summary>
  EventId Id { get; }
}
