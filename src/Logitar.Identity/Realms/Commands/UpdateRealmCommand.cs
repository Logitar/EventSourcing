using MediatR;

namespace Logitar.Identity.Realms.Commands;

/// <summary>
/// The command raised to update an existing realm.
/// </summary>
/// <param name="Id">The identifier of the realm to update.</param>
/// <param name="Input">The update input data.</param>
internal record UpdateRealmCommand(Guid Id, UpdateRealmInput Input) : IRequest<Realm>;
