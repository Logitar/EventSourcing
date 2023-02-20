using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to delete a postal address.
/// </summary>
/// <param name="Id">The identifier of the postal address to delete.</param>
internal record DeleteAddressCommand(Guid Id) : IRequest<Address>;
