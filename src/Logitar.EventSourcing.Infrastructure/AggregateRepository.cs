namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents an interface that allows retrieving and storing events in an event store.
/// </summary>
public abstract class AggregateRepository : IAggregateRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRepository"/> class.
  /// </summary>
  /// <param name="eventBus">The event bus to publish the events to.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  protected AggregateRepository(IEventBus eventBus, IEventSerializer eventSerializer)
  {
    EventBus = eventBus;
    EventSerializer = eventSerializer;
  }

  /// <summary>
  /// Gets the event bus to publish the events to.
  /// </summary>
  protected IEventBus EventBus { get; }
  /// <summary>
  /// Gets the serializer for events.
  /// </summary>
  protected IEventSerializer EventSerializer { get; }

  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    return await LoadAsync<T>(id, includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(AggregateId id, long? version, CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    return await LoadAsync<T>(id, version, includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="includeDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(AggregateId id, bool includeDeleted, CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    return await LoadAsync<T>(id, version: null, includeDeleted, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="includeDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(AggregateId id, long? version, bool includeDeleted, CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    IEnumerable<DomainEvent> changes = await LoadChangesAsync<T>(id, version, cancellationToken);
    return Load<T>(changes, includeDeleted).SingleOrDefault();
  }
  /// <summary>
  /// Loads the events of an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded events.</returns>
  protected abstract Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(AggregateId id, long? version, CancellationToken cancellationToken);

  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IEnumerable<T>> LoadAsync<T>(CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    return await LoadAsync<T>(includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IEnumerable<T>> LoadAsync<T>(bool includeDeleted, CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    IEnumerable<DomainEvent> changes = await LoadChangesAsync<T>(cancellationToken);
    return Load<T>(changes, includeDeleted);
  }
  /// <summary>
  /// Loads the events of all aggregates of the specified type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded events.</returns>
  protected abstract Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(CancellationToken cancellationToken);

  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    return await LoadAsync<T>(ids, includeDeleted: false, cancellationToken);
  }
  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, bool includeDeleted, CancellationToken cancellationToken)
    where T : AggregateRoot
  {
    IEnumerable<DomainEvent> changes = await LoadChangesAsync<T>(ids, cancellationToken);
    return Load<T>(changes, includeDeleted);
  }
  /// <summary>
  /// Loads the events of the specified aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded events.</returns>
  protected abstract Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken);

  /// <summary>
  /// Persists an aggregate to the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public virtual async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
  {
    await SaveAsync(new[] { aggregate }, cancellationToken);
  }
  /// <summary>
  /// Persists a list of aggregates to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public virtual async Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
  {
    await SaveChangesAsync(aggregates, cancellationToken);

    await PublishAndClearChangesAsync(aggregates, cancellationToken);
  }
  /// <summary>
  /// Persists a list of aggregate changes to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist their changes.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected abstract Task SaveChangesAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken);

  /// <summary>
  /// Loads a list of aggregates from the specified events.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="events">The events to load aggregates from.</param>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <returns>The loaded aggregates.</returns>
  protected virtual IEnumerable<T> Load<T>(IEnumerable<DomainEvent> events, bool includeDeleted = false)
    where T : AggregateRoot
  {
    List<T> aggregates = new(events.Count());

    IEnumerable<IGrouping<AggregateId, DomainEvent>> groups = events.GroupBy(e => e.AggregateId);
    foreach (IGrouping<AggregateId, DomainEvent> group in groups)
    {
      T aggregate = AggregateRoot.LoadFromChanges<T>(group.Key, group);
      if (!aggregate.IsDeleted || includeDeleted)
      {
        aggregates.Add(aggregate);
      }
    }

    return aggregates.AsReadOnly();
  }

  /// <summary>
  /// Publishes to the event bus and clear the changes of the specified aggregates.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to publish their events.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected async Task PublishAndClearChangesAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
  {
    foreach (AggregateRoot aggregate in aggregates)
    {
      if (aggregate.HasChanges)
      {
        foreach (DomainEvent change in aggregate.Changes)
        {
          await EventBus.PublishAsync(change, cancellationToken);
        }

        aggregate.ClearChanges();
      }
    }
  }
}
