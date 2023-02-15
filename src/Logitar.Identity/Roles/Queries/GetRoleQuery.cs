using MediatR;

namespace Logitar.Identity.Roles.Queries;

/// <summary>
/// The query raised to retrieve a single role.
/// </summary>
/// <param name="Id">The identifier of the role.</param>
/// <param name="RealmId">The identifier of the realm in which to search the unique name.</param>
/// <param name="UniqueName">The unique name of the role.</param>
internal record GetRoleQuery(Guid? Id, Guid? RealmId, string? UniqueName) : IRequest<Role?>;
