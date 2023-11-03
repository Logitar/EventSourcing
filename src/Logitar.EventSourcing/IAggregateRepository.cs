namespace Logitar.EventSourcing;

/// <summary>
/// Represents an interface that allows retrieving and storing events in an event store.
/// </summary>
public interface IAggregateRepository
{
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, long? version, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="includeDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="includeDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, long? version, bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;

  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;

  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;

  /// <summary>
  /// Persists an aggregate to the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);
  /// <summary>
  /// Persists a list of aggregates to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);
}
