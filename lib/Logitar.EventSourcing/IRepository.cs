namespace Logitar.EventSourcing;

public interface IRepository
{
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
