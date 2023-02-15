using MediatR;

namespace Logitar.Identity.Realms.Commands;

/// <summary>
/// The command raised to delete a realm.
/// </summary>
/// <param name="Id">The identifier of the realm to delete.</param>
internal record DeleteRealmCommand(Guid Id) : IRequest<Realm>;
