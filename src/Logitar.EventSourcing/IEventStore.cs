namespace Logitar.EventSourcing;

public interface IEventStore
{
  Task<T?> LoadAsync<T>(AggregateId id, long? version = null, bool includeDeleted = false, CancellationToken cancellationToken = default) where T : AggregateRoot;
  Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, bool includeDeleted = false, CancellationToken cancellationToken = default) where T : AggregateRoot;

  Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);
}
