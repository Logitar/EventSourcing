using Logitar.Identity.Realms;
using Logitar.Identity.Roles;

namespace Logitar.Identity.Users;

/// <summary>
/// Defines methods to help managing users.
/// </summary>
internal interface IUserHelper
{
  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The user creation input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    CreateUserInput input,
    CancellationToken cancellationToken = default);

  /// <summary>
  /// Resolves a list of roles using the specified arguments.
  /// </summary>
  /// <param name="realm">The realm that the roles should belong to.</param>
  /// <param name="input">The user update input data.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of the roles.</returns>
  Task<IEnumerable<RoleAggregate>?> GetRolesAsync(RealmAggregate realm,
    UpdateUserInput input,
    CancellationToken cancellationToken = default);
}
