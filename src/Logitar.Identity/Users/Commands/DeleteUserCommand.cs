using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The command raised to delete an user.
/// </summary>
/// <param name="Id">The identifier of the user to delete.</param>
internal record DeleteUserCommand(Guid Id) : IRequest<User>;
