using Logitar.EventSourcing.Infrastructure;
using MongoDB.Driver;

namespace Logitar.EventSourcing.MongoDB;

/// <summary>
/// Represents the MongoDB implementation of an interface that allows retrieving and storing events in an event store.
/// </summary>
public class AggregateRepository : Infrastructure.AggregateRepository
{
  /// <summary>
  /// The MongoDB collection of events.
  /// </summary>
  private readonly IMongoCollection<EventEntity> _events;

  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRepository"/> class.
  /// </summary>
  /// <param name="eventBus">The event bus to publish the events to.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  /// <param name="mongoDatabase">The MongoDB database.</param>
  public AggregateRepository(IEventBus eventBus, IEventSerializer eventSerializer,
    IMongoDatabase mongoDatabase) : base(eventBus, eventSerializer)
  {
    _events = mongoDatabase.GetCollection<EventEntity>("events");
  }

  /// <summary>
  /// Loads the events of an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded events.</returns>
  protected override async Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(AggregateId id, long? version, CancellationToken cancellationToken)
  {
    List<FilterDefinition<EventEntity>> filters = new(capacity: 3)
    {
      Builders<EventEntity>.Filter.Eq(x => x.AggregateType, typeof(T).GetName()),
      Builders<EventEntity>.Filter.Eq(x => x.AggregateId, id.Value)
    };
    if (version.HasValue)
    {
      filters.Add(Builders<EventEntity>.Filter.Lte(x => x.Version, version.Value));
    }

    List<EventEntity> events = await _events
      .Find(Builders<EventEntity>.Filter.And(filters.ToArray()))
      .Sort(Builders<EventEntity>.Sort.Ascending(x => x.Version))
      .ToListAsync(cancellationToken);

    return events.Select(EventSerializer.Deserialize);
  }

  /// <summary>
  /// Loads the events of all aggregates of the specified type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded events.</returns>
  protected override async Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(CancellationToken cancellationToken)
  {
    List<EventEntity> events = await _events
      .Find(Builders<EventEntity>.Filter.Eq(x => x.AggregateType, typeof(T).GetName()))
      .Sort(Builders<EventEntity>.Sort.Ascending(x => x.Version))
      .ToListAsync(cancellationToken);

    return events.Select(EventSerializer.Deserialize);
  }

  /// <summary>
  /// Loads the events of the specified aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded events.</returns>
  protected override async Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken)
  {
    HashSet<string> aggregateIds = ids.Select(id => id.Value).ToHashSet();
    List<FilterDefinition<EventEntity>> filters = new(capacity: 3)
    {
      Builders<EventEntity>.Filter.Eq(x => x.AggregateType, typeof(T).GetName()),
      Builders<EventEntity>.Filter.In(x => x.AggregateId, aggregateIds)
    };

    List<EventEntity> events = await _events
      .Find(Builders<EventEntity>.Filter.And(filters.ToArray()))
      .Sort(Builders<EventEntity>.Sort.Ascending(x => x.Version))
      .ToListAsync(cancellationToken);

    return events.Select(EventSerializer.Deserialize);
  }

  /// <summary>
  /// Saves the changes of the specified aggregates to the event store.
  /// </summary>
  /// <param name="aggregates">The aggregates to save the changes.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected override async Task SaveChangesAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
  {
    IEnumerable<EventEntity> events = aggregates.SelectMany(aggregate => EventEntity.FromChanges(aggregate, EventSerializer));
    await _events.InsertManyAsync(events, new InsertManyOptions(), cancellationToken);
  }
}
