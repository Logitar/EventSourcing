namespace Logitar.EventSourcing.Infrastructure;

public class Repository : IRepository
{
  protected IEventStore EventStore { get; }

  public Repository(IEventStore eventStore)
  {
    EventStore = eventStore;
  }

  public virtual async Task<T?> LoadAsync<T>(StreamId id, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(id, version: null, isDeleted: null, cancellationToken);
  }
  public virtual async Task<T?> LoadAsync<T>(StreamId id, long? version, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(id, version, isDeleted: null, cancellationToken);
  }
  public virtual async Task<T?> LoadAsync<T>(StreamId id, bool? isDeleted, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(id, version: null, isDeleted, cancellationToken);
  }
  public virtual async Task<T?> LoadAsync<T>(StreamId id, long? version, bool? isDeleted, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    FetchOptions options = new();
    if (version.HasValue)
    {
      options.ToVersion = version.Value;
    }

    Stream? stream = await EventStore.FetchAsync(id, options, cancellationToken);
    if (stream != null && (!isDeleted.HasValue || isDeleted.Value == stream.IsDeleted))
    {
      return LoadAggregate<T>(stream);
    }

    return null;
  }

  public virtual async Task<IReadOnlyCollection<T>> LoadAsync<T>(CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(isDeleted: null, cancellationToken);
  }
  public virtual Task<IReadOnlyCollection<T>> LoadAsync<T>(bool? isDeleted, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    throw new NotImplementedException();
  }

  public virtual async Task<IReadOnlyCollection<T>> LoadAsync<T>(IEnumerable<StreamId> ids, CancellationToken cancellationToken) where T : class, IAggregate, new()
  {
    return await LoadAsync<T>(ids, isDeleted: null, cancellationToken);
  }
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

  public virtual async Task SaveAsync(IAggregate aggregate, CancellationToken cancellationToken)
  {
    await SaveAsync([aggregate], cancellationToken);
  }

  public virtual async Task SaveAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken)
  {
    foreach (IAggregate aggregate in aggregates)
    {
      if (aggregate.HasChanges)
      {
        long? version = GetVersion(aggregate);
        StreamExpectation expectation = version.HasValue ? StreamExpectation.ShouldBeAtVersion(version.Value) : StreamExpectation.None;
        EventStore.Append(aggregate.Id, aggregate.GetType(), expectation, aggregate.Changes);

        aggregate.ClearChanges();
      }
    }

    await EventStore.SaveChangesAsync(cancellationToken);
  }

  protected virtual long? GetVersion(IAggregate aggregate) => aggregate is IVersionedAggregate versioned ? versioned.Version : null;

  protected virtual T LoadAggregate<T>(Stream stream) where T : IAggregate, new()
  {
    T aggregate = new();

    IEnumerable<IEvent> changes = stream.Events.Select(@event => @event.Data);
    aggregate.LoadFromChanges(stream.Id, changes);

    return aggregate;
  }
}
