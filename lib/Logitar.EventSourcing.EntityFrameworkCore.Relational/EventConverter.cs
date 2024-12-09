using Logitar.EventSourcing.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

public class EventConverter : IEventConverter
{
  protected IEventSerializer Serializer { get; }

  public EventConverter(IEventSerializer serializer)
  {
    Serializer = serializer;
  }

  public virtual EventEntity ToEventEntity(IEvent @event, StreamEntity stream)
  {
    EventId id = GetEventId(@event);
    long? version = GetVersion(@event);
    ActorId? actorId = GetActorId(@event);
    DateTime? occurredOn = GetOccurredOn(@event);
    bool? isDeleted = GetIsDeleted(@event);
    string typeName = GetTypeName(@event);
    string namespacedType = GetNamespacedType(@event);
    string data = GetData(@event);

    return new EventEntity(id, stream, version, actorId, occurredOn, isDeleted, typeName, namespacedType, data);
  }

  protected virtual EventId GetEventId(IEvent @event) => @event is IIdentifiableEvent identifiable ? identifiable.Id : EventId.NewId();

  protected virtual long? GetVersion(IEvent @event) => @event is IVersionedEvent versioned ? versioned.Version : null;

  protected virtual ActorId? GetActorId(IEvent @event) => @event is IActorEvent actor ? actor.ActorId : null;

  protected virtual DateTime? GetOccurredOn(IEvent @event) => @event is ITemporalEvent temporal ? temporal.OccurredOn : null;

  protected virtual bool? GetIsDeleted(IEvent @event)
  {
    if (@event is IDeleteControlEvent control && control.IsDeleted.HasValue)
    {
      return control.IsDeleted.Value;
    }
    else if (@event is IDeleteEvent && @event is not IUndeleteEvent)
    {
      return true;
    }
    else if (@event is IUndeleteEvent && @event is not IDeleteEvent)
    {
      return false;
    }

    return null;
  }

  protected virtual string GetTypeName(IEvent @event) => @event.GetType().Name;

  protected virtual string GetNamespacedType(IEvent @event) => @event.GetType().GetNamespaceQualifiedName();

  protected virtual string GetData(IEvent @event) => Serializer.Serialize(@event);
}
