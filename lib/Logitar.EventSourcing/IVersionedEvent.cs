namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event that is versioned.
/// </summary>
public interface IVersionedEvent : IEvent
{
  /// <summary>
  /// Gets the version of the event.
  /// </summary>
  long Version { get; }
}
