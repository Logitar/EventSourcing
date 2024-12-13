namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a serializer for events.
/// </summary>
public interface IEventSerializer
{
  /// <summary>
  /// Deserializes the specified event.
  /// </summary>
  /// <param name="type">The type of the event.</param>
  /// <param name="value">The string representation of the event.</param>
  /// <returns>The deserialized event.</returns>
  IEvent Deserialize(Type type, string value);
  /// <summary>
  /// Serializes the specified event.
  /// </summary>
  /// <param name="event">The event to serialize.</param>
  /// <returns>The resulting JSON.</returns>
  string Serialize(IEvent @event);
}
