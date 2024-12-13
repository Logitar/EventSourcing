using EventStore.Client;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Represents a converter, used to convert between event abstractions and EventSoreDB/Kurrent events.
/// </summary>
public interface IEventConverter
{
  /// <summary>
  /// Returns the type associated to the stream in which the specified event resides, if any.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <returns>The stream type, or null.</returns>
  Type? GetStreamType(EventRecord record);
  /// <summary>
  /// Converts the specified EventStoreDB/Kurrent event record to an event.
  /// </summary>
  /// <param name="record">The event record to convert.</param>
  /// <returns>The resulting event.</returns>
  Event ToEvent(EventRecord record);

  /// <summary>
  /// Converts the specified event to EventStoreDB/Kurrent event data.
  /// </summary>
  /// <param name="event">The event to convert.</param>
  /// <param name="streamType">The type of the stream in which the event resides.</param>
  /// <returns>The resulting event data.</returns>
  EventData ToEventData(IEvent @event, Type? streamType = null);
}
