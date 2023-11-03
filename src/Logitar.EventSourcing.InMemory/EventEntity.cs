using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.InMemory;

public class EventEntity : IEventEntity
{
  private EventEntity()
  {
  }

  public Guid Id { get; private set; }

  public long Version { get; private set; }

  public string AggregateType { get; private set; } = string.Empty;
  public string AggregateId { get; private set; } = string.Empty;

  public string EventType { get; private set; } = string.Empty;
  public string EventData { get; private set; } = string.Empty;

  public static IEnumerable<EventEntity> FromChanges(AggregateRoot aggregate, IEventSerializer eventSerializer)
  {
    string aggregateId = aggregate.Id.Value;
    string aggregateType = aggregate.GetType().GetName();

    return aggregate.Changes.Select(change => new EventEntity
    {
      Id = change.Id,
      Version = change.Version,
      AggregateType = aggregateType,
      AggregateId = aggregateId,
      EventType = change.GetType().GetName(),
      EventData = eventSerializer.Serialize(change)
    });
  }
}
