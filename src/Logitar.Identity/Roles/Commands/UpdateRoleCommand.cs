using MediatR;

namespace Logitar.Identity.Roles.Commands;

/// <summary>
/// The command raised to update an existing role.
/// </summary>
/// <param name="Id">The identifier of the role to update.</param>
/// <param name="Input">The update input data.</param>
internal record UpdateRoleCommand(Guid Id, UpdateRoleInput Input) : IRequest<Role>;
