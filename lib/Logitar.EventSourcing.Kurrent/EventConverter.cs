using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Implements a converter for event classes.
/// </summary>
public class EventConverter : IEventConverter
{
  /// <summary>
  /// The event serializer.
  /// </summary>
  private readonly IEventSerializer _serializer;

  /// <summary>
  /// Initializes a new instance of the <see cref="EventConverter"/> class.
  /// </summary>
  /// <param name="serializer">The event serializer.</param>
  public EventConverter(IEventSerializer serializer)
  {
    _serializer = serializer;
  }

  /// <summary>
  /// Converts the specified event to an instance of the <see cref="EventData"/> class.
  /// </summary>
  /// <param name="event">The event to convert.</param>
  /// <returns>The converted event.</returns>
  public virtual EventData ToEventData(object @event)
  {
    Uuid eventId = GetEventId(@event);
    string type = GetEventType(@event);
    ReadOnlyMemory<byte> data = GetEventData(@event);
    ReadOnlyMemory<byte>? metadata = GetEventMetadata(@event);

    return new EventData(eventId, type, data, metadata);
  }

  /// <summary>
  /// Returns the identifier of the specified event.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event identifier.</returns>
  protected virtual Uuid GetEventId(object @event)
  {
    if (@event is IIdentifiableEvent identifiable)
    {
      try
      {
        return Uuid.FromGuid(identifiable.Id.ToGuid());
      }
      catch (Exception)
      {
      }
    }

    return Uuid.NewUuid();
  }

  /// <summary>
  /// Returns the type of the specified event. This should be a human-readable value which can be understood by domain experts.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event type.</returns>
  protected virtual string GetEventType(object @event) => @event.GetType().Name;

  /// <summary>
  /// Returns the event data as a contiguous region of memory.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event data.</returns>
  protected virtual ReadOnlyMemory<byte> GetEventData(object @event)
  {
    string serialized = _serializer.Serialize(@event);
    return Encoding.UTF8.GetBytes(serialized);
  }

  /// <summary>
  /// Returns the event metadata as a contiguous region of memory.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event metadata.</returns>
  protected virtual ReadOnlyMemory<byte>? GetEventMetadata(object @event)
  {
    string? streamType = null; // TODO(fpion): implement
    string eventType = @event.GetType().GetNamespaceQualifiedName();
    long version = default; // TODO(fpion): implement

    string? actorId = null; // TODO(fpion): implement
    DateTime occurredOn = default; // TODO(fpion): implement
    bool? isDeleted = null; // TODO(fpion): implement

    EventMetadata metadata = new(streamType, eventType, version, actorId, occurredOn, isDeleted);

    string json = JsonSerializer.Serialize(metadata);
    return Encoding.UTF8.GetBytes(json);
  }
}
