using Logitar.Identity.Roles.Commands;
using Logitar.Identity.Roles.Queries;

namespace Logitar.Identity.Roles;

/// <summary>
/// Implements methods to manage roles in the identity system.
/// </summary>
internal class RoleService : IRoleService
{
  /// <summary>
  /// The request pipeline.
  /// </summary>
  private readonly IRequestPipeline _requestPipeline;

  /// <summary>
  /// Initializes a new instance of the <see cref="RoleService"/> class using the specified arguments.
  /// </summary>
  /// <param name="requestPipeline">The request pipeline.</param>
  public RoleService(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  /// <summary>
  /// Creates a new role.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created role.</returns>
  public async Task<Role> CreateAsync(CreateRoleInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new CreateRoleCommand(input), cancellationToken);
  }

  /// <summary>
  /// Deletes a role.
  /// </summary>
  /// <param name="id">The identifier of the role.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted role.</returns>
  public async Task<Role> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DeleteRoleCommand(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a role by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the role.</param>
  /// <param name="realm">The identifier or unique name of the realm in which to search the unique name.</param>
  /// <param name="uniqueName">The unique name of the role.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role, or null if not found.</returns>
  public async Task<Role?> GetAsync(Guid? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetRoleQuery(id, realm, uniqueName), cancellationToken);
  }

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
  /// <returns>The role list, or empty if none found.</returns>
  public async Task<PagedList<Role>> GetAsync(string? realm, string? search, RoleSort? sort, bool isDescending,
    int? skip, int? take, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetRolesQuery(realm, search,
      sort, isDescending, skip, take), cancellationToken);
  }

  /// <summary>
  /// Updates a role.
  /// </summary>
  /// <param name="id">The identifier of the role.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated role.</returns>
  public async Task<Role> UpdateAsync(Guid id, UpdateRoleInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new UpdateRoleCommand(id, input), cancellationToken);
  }
}
