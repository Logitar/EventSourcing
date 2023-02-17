using Logitar.Identity.Realms;

namespace Logitar.Identity.Roles;

/// <summary>
/// Exposes methods to load roles from the event store.
/// </summary>
public interface IRoleRepository
{
  /// <summary>
  /// Retrieves a role by its realm and unique name.
  /// </summary>
  /// <param name="realm">The realm of the role.</param>
  /// <param name="uniqueName">The unique name of the role.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  Task<RoleAggregate?> LoadAsync(RealmAggregate realm, string uniqueName, CancellationToken cancellationToken = default);
}
