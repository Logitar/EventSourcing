using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to verify an existing phone number.
/// </summary>
/// <param name="Id">The identifier of the phone number to verify.</param>
internal record VerifyPhoneCommand(Guid Id) : IRequest<Phone>;
