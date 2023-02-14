namespace Logitar.EventSourcing;

/// <summary>
/// Represents an event store, used to save new events and load aggregates from their events.
/// </summary>
public interface IEventStore
{
  /// <summary>
  /// Loads an aggregate of the specified type by its aggregate identifier, up to the specified version.
  /// </summary>
  /// <typeparam name="T">The aggregate type</typeparam>
  /// <param name="id">The aggregate identifier</param>
  /// <param name="version">The aggregate version</param>
  /// <param name="includeDeleted">A value indicating whether or not the aggregate should be loaded if it is deleted</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The loaded aggregate or null if none</returns>
  Task<T?> LoadAsync<T>(AggregateId id, long? version = null, bool includeDeleted = false, CancellationToken cancellationToken = default) where T : AggregateRoot;


  Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, bool includeDeleted = false, CancellationToken cancellationToken = default) where T : AggregateRoot;

  /// <summary>
  /// Saves the specified aggregate in the event store.
  /// </summary>
  /// <param name="aggregate">The aggregate to save</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The asynchronous operation</returns>
  Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);

  /// <summary>
  /// Saves the specified aggregates in the event store.
  /// </summary>
  /// <param name="aggregates">The aggregates to save</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <returns>The asynchronous operation</returns>
  Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);
}
