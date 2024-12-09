namespace Logitar.EventSourcing;

public class Event : IActorEvent, IDeleteControlEvent, IIdentifiableEvent, ITemporalEvent, IVersionedEvent
{
  public EventId Id { get; }

  public long Version { get; }

  public ActorId? ActorId { get; }
  public DateTime OccurredOn { get; }

  public bool? IsDeleted { get; }

  public Type Type { get; }
  public IEvent Data { get; }

  public Event(EventId id, long version, DateTime occurredOn, Type type, IEvent data, ActorId? actorId = null, bool? isDeleted = null)
  {
    Id = id;

    Version = version;

    ActorId = actorId;
    OccurredOn = occurredOn;

    IsDeleted = isDeleted;

    Type = type;
    Data = data;
  }

  public override bool Equals(object obj) => obj is Event @event && @event.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Type} | {GetType()} (Id={Id})";
}
