namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

public class EventEntity
{
  private EventEntity()
  {
  }

  public long EventId { get; private set; }
  public Guid Id { get; private set; }
  public long Version { get; private set; }

  public string ActorId { get; private set; } = string.Empty;
  public DateTime OccurredOn { get; private set; }

  public DeleteAction DeleteAction { get; private set; }

  public string AggregateType { get; private set; } = string.Empty;
  public string AggregateId { get; private set; } = string.Empty;

  public string EventType { get; private set; } = string.Empty;
  public string EventData { get; private set; } = string.Empty;

  public static IEnumerable<EventEntity> FromChanges(AggregateRoot aggregate)
  {
    string aggregateType = aggregate.GetType().GetName();

    return aggregate.Changes.Select(change => new EventEntity
    {
      Id = change.Id,
      Version = change.Version,
      ActorId = change.ActorId.Value,
      OccurredOn = change.OccurredOn,
      DeleteAction = change.DeleteAction,
      AggregateType = aggregateType,
      AggregateId = change.AggregateId.Value,
      EventType = change.GetType().GetName(),
      EventData = EventSerializer.Instance.Serialize(change)
    });
  }
}
