namespace Logitar.EventSourcing;

public record DomainEvent : IActorEvent, IDeleteControlEvent, IIdentifiableEvent, IStreamEvent, ITemporalEvent, IVersionedEvent
{
  public EventId Id { get; set; } = EventId.NewId();

  public StreamId StreamId { get; set; }
  public long Version { get; set; }

  public ActorId? ActorId { get; set; }
  public DateTime OccurredOn { get; set; } = DateTime.Now;

  public bool? IsDeleted { get; set; }
}
