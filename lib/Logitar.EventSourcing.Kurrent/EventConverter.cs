using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Represents a converter, used to convert between event abstractions and EventSoreDB/Kurrent events.
/// </summary>
public class EventConverter : IEventConverter
{
  /// <summary>
  /// Gets the event serializer.
  /// </summary>
  protected IEventSerializer Serializer { get; }
  /// <summary>
  /// Gets the metadata serializer options.
  /// </summary>
  protected JsonSerializerOptions SerializerOptions { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventConverter"/> class.
  /// </summary>
  /// <param name="serializer">The event serializer.</param>
  public EventConverter(IEventSerializer serializer)
  {
    Serializer = serializer;

    SerializerOptions = new JsonSerializerOptions();
    RegisterConverters();
  }

  /// <summary>
  /// Registers the converters used to (de)serialize event metadata.
  /// </summary>
  protected virtual void RegisterConverters()
  {
    SerializerOptions.Converters.Add(new ActorIdConverter());
    SerializerOptions.Converters.Add(new EventIdConverter());
    SerializerOptions.Converters.Add(new TypeConverter());
  }

  /// <summary>
  /// Returns the type associated to the stream in which the specified event resides, if any.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <returns>The stream type, or null.</returns>
  public virtual Type? GetStreamType(EventRecord record)
  {
    EventMetadata? metadata = GetEventMetadata<EventMetadata>(record);
    return metadata?.StreamType;
  }

  /// <summary>
  /// Converts the specified EventStoreDB/Kurrent event record to an event.
  /// </summary>
  /// <param name="record">The event record to convert.</param>
  /// <returns>The resulting event.</returns>
  public virtual Event ToEvent(EventRecord record)
  {
    EventMetadata? metadata = GetEventMetadata<EventMetadata>(record);

    EventId id = GetEventId(record, metadata);
    long version = GetVersion(record, metadata);
    DateTime occurredOn = GetOccurredOn(record, metadata);
    Type type = GetEventType(record, metadata);
    IEvent data = GetEventData(record, type, metadata);
    ActorId? actorId = GetActorId(record, metadata);
    bool? isDeleted = GetIsDeleted(record, metadata);

    return new Event(id, version, occurredOn, data, actorId, isDeleted);
  }

  /// <summary>
  /// Converts the specified event to EventStoreDB/Kurrent event data.
  /// </summary>
  /// <param name="event">The event to convert.</param>
  /// <param name="streamType">The type of the stream in which the event resides.</param>
  /// <returns>The resulting event data.</returns>
  public virtual EventData ToEventData(IEvent @event, Type? streamType)
  {
    Uuid eventId = GetEventId(@event);
    string type = GetEventType(@event);
    ReadOnlyMemory<byte> data = GetEventData(@event);
    ReadOnlyMemory<byte> metadata = GetEventMetadata(@event, streamType);
    string contentType = GetContentType(@event);

    return new EventData(eventId, type, data, metadata, contentType);
  }

  /// <summary>
  /// Returns the identifier of the specified event.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event identifier.</returns>
  protected virtual Uuid GetEventId(IEvent @event)
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
  /// Returns the identifier of the specified event.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <param name="metadata">The event metadata.</param>
  /// <returns>The event identifier.</returns>
  protected virtual EventId GetEventId(EventRecord record, EventMetadata? metadata) => metadata?.EventId ?? new EventId(record.EventId.ToGuid());

  /// <summary>
  /// Returns a string representation of the event type.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event type represented as a string.</returns>
  protected virtual string GetEventType(IEvent @event) => @event.GetType().Name;
  /// <summary>
  /// Returns the type of the specified event.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <param name="metadata">The event metadata.</param>
  /// <returns>The event type.</returns>
  /// <exception cref="EventTypeNotFoundException">The event type could not be resolved.</exception>
  protected virtual Type GetEventType(EventRecord record, EventMetadata? metadata)
  {
    return metadata?.EventType ?? Type.GetType(record.EventType) ?? throw new EventTypeNotFoundException(record);
  }

  /// <summary>
  /// Returns the event data as a contiguous region of memory.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The event data.</returns>
  protected virtual ReadOnlyMemory<byte> GetEventData(IEvent @event)
  {
    string json = Serializer.Serialize(@event);
    return Encoding.UTF8.GetBytes(json);
  }
  /// <summary>
  /// Returns the data of the specified event.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <param name="type">The event type.</param>
  /// <param name="metadata">The event metadata.</param>
  /// <returns>The event.</returns>
  protected virtual IEvent GetEventData(EventRecord record, Type type, EventMetadata? metadata)
  {
    string json = Encoding.UTF8.GetString(record.Data.ToArray());
    return Serializer.Deserialize(type, json);
  }

  /// <summary>
  /// Returns the event metadata as a contiguous region of memory.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <param name="streamType">The type of the stream in which the event resides.</param>
  /// <returns>The event metadata.</returns>
  protected virtual ReadOnlyMemory<byte> GetEventMetadata(IEvent @event, Type? streamType)
  {
    EventId? eventId = @event is IIdentifiableEvent identifiable ? identifiable.Id : null;
    long? version = @event is IVersionedEvent versioned ? versioned.Version : null;
    ActorId? actorId = @event is IActorEvent actor ? actor.ActorId : null;
    DateTime? occurredOn = @event is ITemporalEvent temporal ? temporal.OccurredOn : null;

    bool? isDeleted = null;
    if (@event is IDeleteControlEvent control && control.IsDeleted.HasValue)
    {
      isDeleted = control.IsDeleted.Value;
    }
    else if (@event is IDeleteEvent && @event is not IUndeleteEvent)
    {
      isDeleted = true;
    }
    else if (@event is IUndeleteEvent && @event is not IDeleteEvent)
    {
      isDeleted = false;
    }

    EventMetadata metadata = new(@event.GetType(), eventId, streamType, version, actorId, occurredOn, isDeleted);

    string json = JsonSerializer.Serialize(metadata, SerializerOptions);
    return Encoding.UTF8.GetBytes(json);
  }
  /// <summary>
  /// Returns the metadata of the specified event.
  /// </summary>
  /// <typeparam name="T">The type of the metadata object.</typeparam>
  /// <param name="record">The event record.</param>
  /// <returns>THe metadata object.</returns>
  protected virtual T? GetEventMetadata<T>(EventRecord record)
  {
    if (record.Metadata.IsEmpty)
    {
      return default;
    }

    string json = Encoding.UTF8.GetString(record.Metadata.ToArray());
    return JsonSerializer.Deserialize<T>(json, SerializerOptions);
  }

  /// <summary>
  /// Returns the content-type of the specified event's data.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <returns>The content-type.</returns>
  protected virtual string GetContentType(IEvent @event) => MediaTypeNames.Application.Json;

  /// <summary>
  /// Returns the version of the specified event.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <param name="metadata">The event metadata.</param>
  /// <returns></returns>
  protected virtual long GetVersion(EventRecord record, EventMetadata? metadata) => metadata?.Version ?? (record.EventNumber.ToInt64() + 1);

  /// <summary>
  /// Returns the date and time when the specified event occurred.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <param name="metadata">The event metadata.</param>
  /// <returns>The date and time.</returns>
  protected virtual DateTime GetOccurredOn(EventRecord record, EventMetadata? metadata) => metadata?.OccurredOn ?? record.Created;

  /// <summary>
  /// Returns the identifier of the actor who raised the event.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <param name="metadata">The event metadata.</param>
  /// <returns>The actor identifier.</returns>
  protected virtual ActorId? GetActorId(EventRecord record, EventMetadata? metadata) => metadata?.ActorId;

  /// <summary>
  /// Returns a value indicating whether the event stream is deleted, undeleted, or deletion status is unchanged by the specified event.
  /// </summary>
  /// <param name="record">The event record.</param>
  /// <param name="metadata">The event metadata.</param>
  /// <returns>The boolean value.</returns>
  protected virtual bool? GetIsDeleted(EventRecord record, EventMetadata? metadata) => metadata?.IsDeleted;
}
