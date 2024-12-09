using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using System.Text.Json;

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

  protected virtual string GetEventType(IEvent @event) => @event.GetType().Name;

  protected virtual ReadOnlyMemory<byte> GetEventData(IEvent @event)
  {
    string json = Serializer.Serialize(@event);
    return Encoding.UTF8.GetBytes(json);
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

  protected virtual string GetContentType(IEvent @event) => MediaTypeNames.Application.Json;
}
