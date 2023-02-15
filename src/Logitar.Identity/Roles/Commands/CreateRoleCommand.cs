using MediatR;

namespace Logitar.Identity.Roles.Commands;

/// <summary>
/// The command raised to create a new role.
/// </summary>
/// <param name="Input">The creation input data.</param>
internal record CreateRoleCommand(CreateRoleInput Input) : IRequest<Role>;
