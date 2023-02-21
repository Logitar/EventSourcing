using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to set default an existing phone number.
/// </summary>
/// <param name="Id">The identifier of the phone number to set default.</param>
internal record SetDefaultPhoneCommand(Guid Id) : IRequest<Phone>;
