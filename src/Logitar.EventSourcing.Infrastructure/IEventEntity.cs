namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a storage model for events.
/// </summary>
public interface IEventEntity
{
  /// <summary>
  /// Gets the identifier of the event.
  /// </summary>
  Guid Id { get; }

  /// <summary>
  /// Gets the type of the event.
  /// </summary>
  string EventType { get; }
  /// <summary>
  /// Gets the data of the event.
  /// </summary>
  string EventData { get; }
}
