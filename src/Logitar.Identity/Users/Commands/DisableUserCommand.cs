using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The command raised to disable an user account.
/// </summary>
/// <param name="Id">The identifier of the user to disable.</param>
internal record DisableUserCommand(Guid Id) : IRequest<User>;
