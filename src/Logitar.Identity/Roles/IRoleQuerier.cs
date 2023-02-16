using Logitar.EventSourcing;

namespace Logitar.Identity.Roles;

/// <summary>
/// Exposes methods used to query role read models.
/// </summary>
public interface IRoleQuerier
{
  /// <summary>
  /// Retrieves a role by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  Task<Role?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a role by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  Task<Role?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Retrieves a role by its realm and unique name.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm in which to search the unique name.</param>
  /// <param name="uniqueName">The unique name.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  Task<Role?> GetAsync(string realm, string uniqueName, CancellationToken cancellationToken = default);
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
}
