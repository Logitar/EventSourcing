using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents an interface that allows retrieving and storing events in an Entity Framework Core relational event store.
/// </summary>
public class AggregateRepository : Infrastructure.AggregateRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRepository"/> class.
  /// </summary>
  /// <param name="eventBus">The event bus.</param>
  /// <param name="eventContext">The event database context.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  public AggregateRepository(IEventBus eventBus, EventContext eventContext,
    IEventSerializer eventSerializer) : base(eventBus, eventSerializer)
  {
    EventContext = eventContext;
  }

  /// <summary>
  /// Gets the event database context.
  /// </summary>
  protected EventContext EventContext { get; }

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
    string aggregateType = typeof(T).GetName();
    string aggregateId = id.Value;

    EventEntity[] events = await EventContext.Events.AsNoTracking()
      .Where(e => e.AggregateType == aggregateType && e.AggregateId == aggregateId && (!version.HasValue || e.Version <= version.Value))
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

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
    string aggregateType = typeof(T).GetName();

    EventEntity[] events = await EventContext.Events.AsNoTracking()
      .Where(e => e.AggregateType == aggregateType)
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

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
    string aggregateType = typeof(T).GetName();
    HashSet<string> aggregateIds = ids.Select(id => id.Value).ToHashSet();

    EventEntity[] events = await EventContext.Events.AsNoTracking()
      .Where(e => e.AggregateType == aggregateType && aggregateIds.Contains(e.AggregateId))
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

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

    EventContext.Events.AddRange(events);
    await EventContext.SaveChangesAsync(cancellationToken);
  }
}
