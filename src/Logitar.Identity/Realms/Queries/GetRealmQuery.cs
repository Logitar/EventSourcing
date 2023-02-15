using MediatR;

namespace Logitar.Identity.Realms.Queries;

/// <summary>
/// The query raised to retrieve a single realm.
/// </summary>
/// <param name="Id">The identifier of the realm.</param>
/// <param name="UniqueName">The unique name of the realm.</param>
internal record GetRealmQuery(Guid? Id, string? UniqueName) : IRequest<Realm?>;
