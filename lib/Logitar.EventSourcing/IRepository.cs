namespace Logitar.EventSourcing;

public interface IRepository
{
  Task<T?> LoadAsync<T>(StreamId id, CancellationToken cancellationToken = default) where T : class, IAggregate;
  Task<T?> LoadAsync<T>(StreamId id, long? version, CancellationToken cancellationToken = default) where T : class, IAggregate;
  Task<T?> LoadAsync<T>(StreamId id, bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate;
  Task<T?> LoadAsync<T>(StreamId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate;

  Task<IReadOnlyCollection<T>> LoadAsync<T>(CancellationToken cancellationToken = default) where T : class, IAggregate;
  Task<IReadOnlyCollection<T>> LoadAsync<T>(bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate;

  Task<IReadOnlyCollection<T>> LoadAsync<T>(IEnumerable<StreamId> ids, CancellationToken cancellationToken = default) where T : class, IAggregate;
  Task<IReadOnlyCollection<T>> LoadAsync<T>(IEnumerable<StreamId> ids, bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate;

  Task SaveAsync(IAggregate aggregate, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken = default);
}
