namespace Logitar.EventSourcing;

/// <summary>
/// Implements a repository that allows retrieving and storing aggregates in an event store.
/// </summary>
public class Repository : IRepository
{
  /// <summary>
  /// Gets the event store.
  /// </summary>
  protected IEventStore EventStore { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Repository"/> class.
  /// </summary>
  /// <param name="eventStore">The event store.</param>
  public Repository(IEventStore eventStore)
  {
    EventStore = eventStore;
  }

  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(StreamId id, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(id, version: null, isDeleted: null, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(StreamId id, long? version, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(id, version, isDeleted: null, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="isDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(StreamId id, bool? isDeleted, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(id, version: null, isDeleted, cancellationToken);
  }
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="isDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  public virtual async Task<T?> LoadAsync<T>(StreamId id, long? version, bool? isDeleted, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    FetchOptions options = new();
    if (version.HasValue)
    {
      options.ToVersion = version.Value;
    }
    if (isDeleted.HasValue)
    {
      options.IsDeleted = isDeleted.Value;
    }

    Stream? stream = await EventStore.FetchAsync(id, options, cancellationToken);
    return stream == null ? null : LoadAggregate<T>(stream);
  }

  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IReadOnlyCollection<T>> LoadAsync<T>(CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(isDeleted: null, cancellationToken);
  }
  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="isDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IReadOnlyCollection<T>> LoadAsync<T>(bool? isDeleted, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    IReadOnlyCollection<Stream> streams = await EventStore.FetchAsync(new FetchManyOptions(), cancellationToken);
    List<T> aggregates = new(capacity: streams.Count);
    foreach (Stream stream in streams)
    {
      if (stream.Type != null && stream.Type.Equals(typeof(T)) && (!isDeleted.HasValue || isDeleted.Value == stream.IsDeleted))
      {
        T aggregate = LoadAggregate<T>(stream);
        aggregates.Add(aggregate);
      }
    }

    return aggregates.AsReadOnly();
  }

  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IReadOnlyCollection<T>> LoadAsync<T>(IEnumerable<StreamId> ids, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(ids, isDeleted: null, cancellationToken);
  }
  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="isDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  public virtual async Task<IReadOnlyCollection<T>> LoadAsync<T>(IEnumerable<StreamId> ids, bool? isDeleted, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    List<T> aggregates = new(capacity: ids.Count());
    foreach (StreamId id in ids)
    {
      T? aggregate = await LoadAsync<T>(id, isDeleted, cancellationToken);
      if (aggregate != null)
      {
        aggregates.Add(aggregate);
      }
    }

    return aggregates.AsReadOnly();
  }

  /// <summary>
  /// Persists an aggregate to the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public virtual async Task SaveAsync(IAggregate aggregate, CancellationToken cancellationToken)
  {
    await SaveAsync([aggregate], cancellationToken);
  }

  /// <summary>
  /// Persists a list of aggregates to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public virtual async Task SaveAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken)
  {
    int changes = 0;

    foreach (IAggregate aggregate in aggregates)
    {
      if (aggregate.HasChanges)
      {
        changes++;

        long? version = GetVersion(aggregate);
        StreamExpectation expectation = version.HasValue ? StreamExpectation.ShouldBeAtVersion(version.Value) : StreamExpectation.None;
        EventStore.Append(aggregate.Id, aggregate.GetType(), expectation, aggregate.Changes);

        aggregate.ClearChanges();
      }
    }

    if (changes > 0)
    {
      await EventStore.SaveChangesAsync(cancellationToken);
    }
  }

  /// <summary>
  /// Gets the version of the specified aggregate.
  /// </summary>
  /// <param name="aggregate">The aggregate.</param>
  /// <returns>The version, or null if the aggregate is not versioned.</returns>
  protected virtual long? GetVersion(IAggregate aggregate) => aggregate is IVersionedAggregate versioned ? versioned.Version : null;

  /// <summary>
  /// Loads an aggregate from the specified stream of events.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="stream">The event stream.</param>
  /// <returns>The loaded aggregate.</returns>
  protected virtual T LoadAggregate<T>(Stream stream) where T : IAggregate, new()
  {
    T aggregate = new();

    IEnumerable<IEvent> changes = stream.Events.Select(@event => @event.Data);
    aggregate.LoadFromChanges(stream.Id, changes);

    return aggregate;
  }
}
