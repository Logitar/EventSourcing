﻿namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a serializer for events.
/// </summary>
public interface IEventSerializer
{
  /// <summary>
  /// Serializes the specified event.
  /// </summary>
  /// <param name="event">The event to serialize.</param>
  /// <returns>The serialized string representation of the event.</returns>
  string Serialize(IEvent @event);
}
