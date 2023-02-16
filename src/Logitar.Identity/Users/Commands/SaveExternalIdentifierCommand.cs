using MediatR;

namespace Logitar.Identity.Users.Commands;

/// <summary>
/// The command raised to add, remove or update the external identifier of an user.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
/// <param name="Key">The key of the external identifier.</param>
/// <param name="Value">The value of the external identifier. If null, the external identifier will be removed.</param>
internal record SaveExternalIdentifierCommand(Guid Id, string Key, string? Value) : IRequest<User>;
