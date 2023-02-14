namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model used to represent an event.
/// </summary>
public class EventEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventEntity"/> class.
  /// </summary>
  private EventEntity()
  {
  }

  /// <summary>
  /// The sequential identifier of the event
  /// </summary>
  public long EventId { get; private set; }
  /// <summary>
  /// The random identifier of the event
  /// </summary>
  public Guid Id { get; private set; }
  /// <summary>
  /// The version of the event
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// The identifier of the actor who triggered the event
  /// </summary>
  public string ActorId { get; private set; } = string.Empty;
  /// <summary>
  /// The date and time when the event occurred
  /// </summary>
  public DateTime OccurredOn { get; private set; }

  /// <summary>
  /// The delete action performed by the event
  /// </summary>
  public DeleteAction DeleteAction { get; private set; }

  /// <summary>
  /// The type of the aggregate
  /// </summary>
  public string AggregateType { get; private set; } = string.Empty;
  /// <summary>
  /// The identifier of the aggregate
  /// </summary>
  public string AggregateId { get; private set; } = string.Empty;

  /// <summary>
  /// The event type
  /// </summary>
  public string EventType { get; private set; } = string.Empty;
  /// <summary>
  /// The event data as JSON
  /// </summary>
  public string EventData { get; private set; } = string.Empty;

  /// <summary>
  /// Returns a list of event entities from the specified aggregate's changes.
  /// </summary>
  /// <param name="aggregate">The aggregate</param>
  /// <returns>The list of event entities</returns>
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
