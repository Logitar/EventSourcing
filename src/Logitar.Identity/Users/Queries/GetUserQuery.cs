using MediatR;

namespace Logitar.Identity.Users.Queries;

/// <summary>
/// The query raised to retrieve a single user.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
/// <param name="Realm">The identifier or unique name of the realm in which to search the unique name.</param>
/// <param name="Username">The unique name of the user.</param>
internal record GetUserQuery(Guid? Id, string? Realm, string? Username) : IRequest<User?>;
