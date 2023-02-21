using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to create a new phone number.
/// </summary>
/// <param name="Input">The creation input data.</param>
internal record CreatePhoneCommand(CreatePhoneInput Input) : IRequest<Phone>;
