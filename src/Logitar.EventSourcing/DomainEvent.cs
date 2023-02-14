namespace Logitar.EventSourcing;

public abstract record DomainEvent
{
  public Guid Id { get; set; } = Guid.NewGuid();

  public AggregateId AggregateId { get; set; }
  public long Version { get; set; }

  public AggregateId ActorId { get; set; } = new AggregateId("SYSTEM");
  public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

  public DeleteAction DeleteAction { get; set; }
}
