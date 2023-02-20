using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to create a new postal address.
/// </summary>
/// <param name="Input">The creation input data.</param>
internal record CreateAddressCommand(CreateAddressInput Input) : IRequest<Address>;
