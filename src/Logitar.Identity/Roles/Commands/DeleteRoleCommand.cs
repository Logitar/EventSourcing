using MediatR;

namespace Logitar.Identity.Roles.Commands;

/// <summary>
/// The command raised to delete a role.
/// </summary>
/// <param name="Id">The identifier of the role to delete.</param>
internal record DeleteRoleCommand(Guid Id) : IRequest<Role>;
