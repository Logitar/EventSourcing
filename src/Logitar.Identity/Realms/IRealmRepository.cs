using Logitar.EventSourcing;

namespace Logitar.Identity.Realms;

/// <summary>
/// Exposes methods to save and load realms from the event store.
/// </summary>
public interface IRealmRepository
{
  /// <summary>
  /// Retrieves a realm by its identifier.
  /// </summary>
  /// <param name="id">The identifier of the realm.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  Task<RealmAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a realm by its unique name.
  /// </summary>
  /// <param name="uniqueName">The unique name of the realm.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  Task<RealmAggregate?> LoadAsync(string uniqueName, CancellationToken cancellationToken = default);

  /// <summary>
  /// Saves the specified realm in the event store.
  /// </summary>
  /// <param name="realm">The realm to save.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
}
