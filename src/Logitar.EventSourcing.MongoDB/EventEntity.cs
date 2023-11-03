using Logitar.EventSourcing.Infrastructure;
using MongoDB.Bson;

namespace Logitar.EventSourcing.MongoDB;

/// <summary>
/// Represents the MongoDB storage model for events.
/// </summary>
public record EventEntity : IEventEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EventEntity"/> class.
  /// </summary>
  private EventEntity()
  {
  }

  /// <summary>
  /// Gets of sets the internal MongoDB identifier of the event.
  /// </summary>
  public ObjectId EventId { get; private set; }
  /// <summary>
  ///  Gets or sets the identifier of the event.
  /// </summary>
  public Guid Id { get; private set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who triggered the event.
  /// </summary>
  public string ActorId { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets a value indicating whether or not the aggregate is deleted.
  /// </summary>
  public bool? IsDeleted { get; private set; }
  /// <summary>
  /// Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime OccurredOn { get; private set; }
  /// <summary>
  /// Gets or sets the version of the event.
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// Gets or sets the type of the aggregate to which the event belongs to.
  /// </summary>
  public string AggregateType { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets the identifier of the aggregate to which the event belongs to.
  /// </summary>
  public string AggregateId { get; private set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of the event.
  /// </summary>
  public string EventType { get; private set; } = string.Empty;
  /// <summary>
  /// Gets or sets the data of the event.
  /// </summary>
  public string EventData { get; private set; } = string.Empty;

  /// <summary>
  /// Converts the changes of the specified aggregate to the storage model.
  /// </summary>
  /// <param name="aggregate">The aggregate to convert the changes from.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  /// <returns>The list of storage models.</returns>
  public static IEnumerable<EventEntity> FromChanges(AggregateRoot aggregate, IEventSerializer eventSerializer)
  {
    string aggregateId = aggregate.Id.Value;
    string aggregateType = aggregate.GetType().GetName();

    return aggregate.Changes.Select(change => new EventEntity
    {
      Id = change.Id,
      ActorId = change.ActorId.Value,
      IsDeleted = change.IsDeleted,
      OccurredOn = change.OccurredOn.ToUniversalTime(),
      Version = change.Version,
      AggregateType = aggregateType,
      AggregateId = aggregateId,
      EventType = change.GetType().GetName(),
      EventData = eventSerializer.Serialize(change)
    });
  }
}
