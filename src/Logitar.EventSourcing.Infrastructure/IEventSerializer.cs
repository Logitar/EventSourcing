namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a JSON serializer for events.
/// </summary>
public interface IEventSerializer
{
  /// <summary>
  /// Registers the specified JSON converter to the serializer.
  /// </summary>
  /// <param name="converter">The converter to register.</param>
  void RegisterConverter(JsonConverter converter);

  /// <summary>
  /// Deserializes an event from its storage model.
  /// </summary>
  /// <param name="entity">The storage model from which to deserialize.</param>
  /// <returns>The deserialized event.</returns>
  DomainEvent Deserialize(IEventEntity entity);
  /// <summary>
  /// Serializes the specified event to JSON.
  /// </summary>
  /// <param name="change">The event to serialize.</param>
  /// <returns>The resulting JSON.</returns>
  string Serialize(DomainEvent change);
}
