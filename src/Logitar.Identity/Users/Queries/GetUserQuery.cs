using MediatR;

namespace Logitar.Identity.Users.Queries;

/// <summary>
/// The query raised to retrieve a single user.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
/// <param name="Realm">The identifier or unique name of the realm in which to search the unique name.</param>
/// <param name="Username">The unique name of the user.</param>
/// <param name="ExternalKey">The key of an external identifier.</param>
/// <param name="ExternalValue">The value of an external identifier.</param>
internal record GetUserQuery(Guid? Id, string? Realm, string? Username,
  string? ExternalKey, string? ExternalValue) : IRequest<User?>;
