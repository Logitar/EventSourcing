using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to set default an existing postal address.
/// </summary>
/// <param name="Id">The identifier of the postal address to set default.</param>
internal record SetDefaultAddressCommand(Guid Id) : IRequest<Address>;
