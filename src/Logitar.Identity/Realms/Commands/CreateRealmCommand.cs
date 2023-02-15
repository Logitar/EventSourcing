using MediatR;

namespace Logitar.Identity.Realms.Commands;

/// <summary>
/// The command raised to create a new realm.
/// </summary>
/// <param name="Input">The creation input data.</param>
internal record CreateRealmCommand(CreateRealmInput Input) : IRequest<Realm>;
