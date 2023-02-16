namespace Logitar.Identity.Realms;

/// <summary>
/// Exposes methods to load realms from the event store.
/// </summary>
public interface IRealmRepository
{
  /// <summary>
  /// Retrieves a realm by its unique name.
  /// </summary>
  /// <param name="uniqueName">The unique name of the realm.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  Task<RealmAggregate?> LoadAsync(string uniqueName, CancellationToken cancellationToken = default);
}
