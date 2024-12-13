namespace Logitar.EventSourcing;

/// <summary>
/// Represents an interface that allows retrieving and storing aggregates in an event store.
/// </summary>
public interface IRepository
{
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(StreamId id, CancellationToken cancellationToken = default) where T : class, IAggregate, new();
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(StreamId id, long? version, CancellationToken cancellationToken = default) where T : class, IAggregate, new();
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="isDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(StreamId id, bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate, new();
  /// <summary>
  /// Loads an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="isDeleted">A value indicating whether or not a deleted aggregate will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate.</returns>
  Task<T?> LoadAsync<T>(StreamId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate, new();

  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IReadOnlyCollection<T>> LoadAsync<T>(CancellationToken cancellationToken = default) where T : class, IAggregate, new();
  /// <summary>
  /// Loads all the aggregates of a specific type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="isDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IReadOnlyCollection<T>> LoadAsync<T>(bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate, new();

  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IReadOnlyCollection<T>> LoadAsync<T>(IEnumerable<StreamId> ids, CancellationToken cancellationToken = default) where T : class, IAggregate, new();
  /// <summary>
  /// Loads a list of aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="isDeleted">A value indicating whether or not deleted aggregates will be returned.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded aggregates.</returns>
  Task<IReadOnlyCollection<T>> LoadAsync<T>(IEnumerable<StreamId> ids, bool? isDeleted, CancellationToken cancellationToken = default) where T : class, IAggregate, new();

  /// <summary>
  /// Persists an aggregate to the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(IAggregate aggregate, CancellationToken cancellationToken = default);
  /// <summary>
  /// Persists a list of aggregates to the event store.
  /// </summary>
  /// <param name="aggregates">The list of aggregates to persist.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(IEnumerable<IAggregate> aggregates, CancellationToken cancellationToken = default);
}
