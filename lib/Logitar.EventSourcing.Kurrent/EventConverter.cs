using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Implements a converter for event classes.
/// </summary>
public class EventConverter : IEventConverter // TODO(fpion): unit tests
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
  /// <param name="streamType">The type of the event stream.</param>
  /// <returns>The converted event.</returns>
  public virtual EventData ToEventData(object @event, Type? streamType)
  {
    Uuid eventId = GetEventId(@event);
    string type = GetEventType(@event);
    ReadOnlyMemory<byte> data = GetEventData(@event);
    ReadOnlyMemory<byte>? metadata = GetEventMetadata(@event, streamType);

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
  /// <param name="streamType">The type of the event stream.</param>
  /// <returns>The event metadata.</returns>
  protected virtual ReadOnlyMemory<byte>? GetEventMetadata(object @event, Type? streamType)
  {
    string eventType = @event.GetType().GetNamespaceQualifiedName();
    string? eventId = @event is IIdentifiableEvent identifiable ? identifiable.Id.Value : null;

    string? streamTypeName = streamType?.GetNamespaceQualifiedName();
    long version = default; // TODO(fpion): implement

    string? actorId = @event is IActorEvent actor ? actor.ActorId?.Value : null;
    DateTime? occurredOn = @event is ITemporalEvent temporal ? temporal.OccurredOn : null;

    bool? isDeleted = null;
    if (@event is IDeleteControlEvent deleteControl)
    {
      isDeleted = deleteControl.IsDeleted;
    }
    else if (@event is IDeleteEvent && @event is not IUndeleteEvent)
    {
      isDeleted = true;
    }
    else if (@event is IUndeleteEvent && @event is not IDeleteEvent)
    {
      isDeleted = false;
    }

    EventMetadata metadata = new(eventType, eventId, streamTypeName, version, actorId, occurredOn, isDeleted);

    string json = JsonSerializer.Serialize(metadata);
    return Encoding.UTF8.GetBytes(json);
  }
}
