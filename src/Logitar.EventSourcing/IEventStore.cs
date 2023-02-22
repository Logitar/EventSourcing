namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event store, used to save new events and load aggregates from their events.
/// </summary>
public interface IEventStore
{
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier to its most recent version.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregate or null if none.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier, up to the specified version.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="version">The aggregate version.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate or null if none.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, long version, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier to its most recent version.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="version">The aggregate version.</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate or null if none.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier, up to the specified version.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="version">The aggregate version.</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregate or null if none.</returns>
  Task<T?> LoadAsync<T>(AggregateId id, long? version, bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;

  /// <summary>
  /// Loads all the aggregates of the specified type.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregates or an empty collection.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads all the aggregates of the specified type.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregates or an empty collection.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;

  /// <summary>
  /// Loads a list of aggregates of the specified type by their aggregate identifier.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="ids">The aggregate identifiers.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregates or an empty collection.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken = default) where T : AggregateRoot;
  /// <summary>
  /// Loads a list of aggregates of the specified type by their aggregate identifier.
  /// </summary>
  /// <typeparam name="T">The aggregate type.</typeparam>
  /// <param name="ids">The aggregate identifiers.</param>
  /// <param name="includeDeleted">A value indicating whether or not deleted aggregates should be loaded.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded aggregates or an empty collection.</returns>
  Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, bool includeDeleted, CancellationToken cancellationToken = default) where T : AggregateRoot;

  /// <summary>
  /// Saves the specified aggregate in the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to save.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);

  /// <summary>
  /// Saves the specified aggregates in the event store.
  /// </summary>
  /// <param name="aggregates">The aggregates to save.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);
}
