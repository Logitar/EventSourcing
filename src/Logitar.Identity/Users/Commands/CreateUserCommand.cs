using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The command raised to create a new user.
/// </summary>
/// <param name="Input">The creation input data.</param>
internal record CreateUserCommand(CreateUserInput Input) : IRequest<User>;
