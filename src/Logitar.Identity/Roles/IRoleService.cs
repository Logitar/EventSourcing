namespace Logitar.Identity.Roles;

/// <summary>
/// Exposes methods to manage roles in the identity system.
/// </summary>
public interface IRoleService
{
  /// <summary>
  /// Creates a new role.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created role.</returns>
  Task<Role> CreateAsync(CreateRoleInput input, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes a role.
  /// </summary>
  /// <param name="id">The identifier of the role.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted role.</returns>
  Task<Role> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a role by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the role.</param>
  /// <param name="realm">The identifier or unique name of the realm in which to search the unique name.</param>
  /// <param name="uniqueName">The unique name of the role.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role, or null if not found.</returns>
  Task<Role?> GetAsync(Guid? id = null, string? realm = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a list of roles using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm to filter by.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of roles to skip.</param>
  /// <param name="take">The number of roles to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of roles, or empty if none found.</returns>
  Task<PagedList<Role>> GetAsync(string? realm = null, string? search = null, RoleSort? sort = null, bool isDescending = false,
    int? skip = null, int? take = null, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates a role.
  /// </summary>
  /// <param name="id">The identifier of the role.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated role.</returns>
  Task<Role> UpdateAsync(Guid id, UpdateRoleInput input, CancellationToken cancellationToken = default);
}
