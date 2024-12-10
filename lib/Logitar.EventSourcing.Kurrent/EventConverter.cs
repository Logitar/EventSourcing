using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

public class EventConverter : IEventConverter
{
  protected IEventSerializer Serializer { get; }
  protected JsonSerializerOptions SerializerOptions { get; }

  public EventConverter(IEventSerializer serializer)
  {
    Serializer = serializer;

    SerializerOptions = new JsonSerializerOptions();
    RegisterConverters();
  }

  protected virtual void RegisterConverters()
  {
    SerializerOptions.Converters.Add(new ActorIdConverter());
    SerializerOptions.Converters.Add(new EventIdConverter());
    SerializerOptions.Converters.Add(new TypeConverter());
  }

  public virtual Type? GetStreamType(EventRecord record)
  {
    EventMetadata? metadata = GetEventMetadata<EventMetadata>(record);
    return metadata?.StreamType;
  }

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

  public virtual EventData ToEventData(IEvent @event, Type? streamType)
  {
    Uuid eventId = GetEventId(@event);
    string type = GetEventType(@event);
    ReadOnlyMemory<byte> data = GetEventData(@event);
    ReadOnlyMemory<byte> metadata = GetEventMetadata(@event, streamType);
    string contentType = GetContentType(@event);

    return new EventData(eventId, type, data, metadata, contentType);
  }

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

  protected virtual EventId GetEventId(EventRecord record, EventMetadata? metadata) => metadata?.EventId ?? new EventId(record.EventId.ToGuid());

  protected virtual string GetEventType(IEvent @event) => @event.GetType().Name;

  protected virtual Type GetEventType(EventRecord record, EventMetadata? metadata)
  {
    return metadata?.EventType ?? Type.GetType(record.EventType) ?? throw new NotImplementedException();
  }

  protected virtual ReadOnlyMemory<byte> GetEventData(IEvent @event)
  {
    string json = Serializer.Serialize(@event);
    return Encoding.UTF8.GetBytes(json);
  }

  protected virtual IEvent GetEventData(EventRecord record, Type type, EventMetadata? metadata)
  {
    string json = Encoding.UTF8.GetString(record.Data.ToArray());
    return Serializer.Deserialize(type, json);
  }

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

  protected virtual T? GetEventMetadata<T>(EventRecord record)
  {
    string json = Encoding.UTF8.GetString(record.Metadata.ToArray());
    return JsonSerializer.Deserialize<T>(json, SerializerOptions);
  }

  protected virtual string GetContentType(IEvent @event) => MediaTypeNames.Application.Json;

  protected virtual long GetVersion(EventRecord record, EventMetadata? metadata) => metadata?.Version ?? (record.EventNumber.ToInt64() + 1);

  protected virtual DateTime GetOccurredOn(EventRecord record, EventMetadata? metadata) => metadata?.OccurredOn ?? record.Created;

  protected virtual ActorId? GetActorId(EventRecord record, EventMetadata? metadata) => metadata?.ActorId;

  protected virtual bool? GetIsDeleted(EventRecord record, EventMetadata? metadata) => metadata?.IsDeleted;
}
