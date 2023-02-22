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
  /// Gets or sets the sequential identifier of the event.
  /// </summary>
  public long EventId { get; private set; }
  /// <summary>
  /// Gets or sets the random identifier of the event.
  /// </summary>
  public Guid Id { get; private set; }
  /// <summary>
  /// Gets or sets the version of the event.
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who triggered the event.
  /// </summary>
  public string ActorId { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime OccurredOn { get; private set; }

  /// <summary>
  /// Gets or sets the delete action performed by the event.
  /// </summary>
  public DeleteAction DeleteAction { get; private set; }

  /// <summary>
  /// Gets or sets the type of the aggregate owning the event.
  /// </summary>
  public string AggregateType { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets the identifier of the aggregate owning the event.
  /// </summary>
  public string AggregateId { get; private set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of the event.
  /// </summary>
  public string EventType { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets the JSON serialized data of the event.
  /// </summary>
  public string EventData { get; private set; } = string.Empty;

  /// <summary>
  /// Returns a list of event entities from the specified aggregate's changes.
  /// </summary>
  /// <param name="aggregate">The aggregate.</param>
  /// <returns>The list of event entities.</returns>
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
