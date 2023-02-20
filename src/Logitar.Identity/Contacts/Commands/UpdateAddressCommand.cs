using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to update an existing postal address.
/// </summary>
/// <param name="Id">The identifier of the postal address to update.</param>
/// <param name="Input">The update input data.</param>
internal record UpdateAddressCommand(Guid Id, UpdateAddressInput Input) : IRequest<Address>;
