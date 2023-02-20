using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to verify an existing postal address.
/// </summary>
/// <param name="Id">The identifier of the postal address to verify.</param>
internal record VerifyAddressCommand(Guid Id) : IRequest<Address>;
