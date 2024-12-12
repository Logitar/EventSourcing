namespace Logitar.EventSourcing.Demo.Infrastructure.Entities;

internal abstract class AggregateEntity
{
  public string AggregateId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  protected AggregateEntity()
  {
  }

  protected AggregateEntity(DomainEvent @event)
  {
    AggregateId = @event.StreamId.Value;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  protected virtual void Update(DomainEvent @event)
  {
    Version = @event.Version;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is AggregateEntity aggregate && aggregate.AggregateId == AggregateId;
  public override int GetHashCode() => AggregateId.GetHashCode();
  public override string ToString() => $"{GetType()} (AggregateId={AggregateId})";
}
