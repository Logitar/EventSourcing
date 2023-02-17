using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The command raised to enable an user account.
/// </summary>
/// <param name="Id">The identifier of the user to enable.</param>
internal record EnableUserCommand(Guid Id) : IRequest<User>;
