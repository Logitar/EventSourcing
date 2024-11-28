using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.InMemory;

public class EventEntity : IEventEntity
{
  private EventEntity()
  {
  }

  public string Id { get; private set; } = string.Empty;

  public long Version { get; private set; }

  public string AggregateType { get; private set; } = string.Empty;
  public string AggregateId { get; private set; } = string.Empty;

  public string EventType { get; private set; } = string.Empty;
  public string EventData { get; private set; } = string.Empty;

  public static IEnumerable<EventEntity> FromChanges(AggregateRoot aggregate, IEventSerializer eventSerializer)
  {
    string aggregateId = aggregate.Id.Value;
    string aggregateType = aggregate.GetType().GetNamespaceQualifiedName();

    return aggregate.Changes.Select(change => new EventEntity
    {
      Id = change.Id.Value,
      Version = change.Version,
      AggregateType = aggregateType,
      AggregateId = aggregateId,
      EventType = change.GetType().GetNamespaceQualifiedName(),
      EventData = eventSerializer.Serialize(change)
    });
  }
}
