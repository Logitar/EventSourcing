﻿using EventStore.Client;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Defines a converter for event classes.
/// </summary>
public interface IEventConverter
{
  /// <summary>
  /// Converts the specified event record to an instance of the <see cref="Event"/> class.
  /// </summary>
  /// <param name="record">The event to convert.</param>
  /// <returns>The converted event.</returns>
  Event ToEvent(EventRecord record);

  /// <summary>
  /// Converts the specified event to an instance of the <see cref="EventData"/> class.
  /// </summary>
  /// <param name="event">The event to convert.</param>
  /// <param name="streamType">The type of the event stream.</param>
  /// <returns>The converted event.</returns>
  EventData ToEventData(IEvent @event, Type? streamType = null);
}
