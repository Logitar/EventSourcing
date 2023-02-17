using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The command raised to update an existing user.
/// </summary>
/// <param name="Id">The identifier of the user to update.</param>
/// <param name="Input">The update input data.</param>
internal record UpdateUserCommand(Guid Id, UpdateUserInput Input) : IRequest<User>;
