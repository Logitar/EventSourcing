using Logitar.Identity.Realms;

namespace Logitar.Identity.Users;

/// <summary>
/// Exposes methods to load users from the event store.
/// </summary>
public interface IUserRepository
{
  /// <summary>
  /// Retrieves an user by its realm and unique name.
  /// </summary>
  /// <param name="realm">The realm of the user.</param>
  /// <param name="username">The unique name of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  Task<UserAggregate?> LoadAsync(RealmAggregate realm, string username, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves an user by its realm and external identifier.
  /// </summary>
  /// <param name="realm">The realm of the user.</param>
  /// <param name="externalKey">The key of an external identifier.</param>
  /// <param name="externalValue">The value of an external identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  Task<UserAggregate?> LoadAsync(RealmAggregate realm, string externalKey, string externalValue, CancellationToken cancellationToken = default);
}
