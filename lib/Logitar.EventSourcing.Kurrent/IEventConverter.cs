using EventStore.Client;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Defines a converter for event classes.
/// </summary>
public interface IEventConverter
{
  /// <summary>
  /// Converts the specified event to an instance of the <see cref="EventData"/> class.
  /// </summary>
  /// <param name="event">The event to convert.</param>
  /// <returns>The converted event.</returns>
  EventData ToEventData(object @event);
}
