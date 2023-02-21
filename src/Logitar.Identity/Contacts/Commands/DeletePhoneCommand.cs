using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to delete a phone number.
/// </summary>
/// <param name="Id">The identifier of the phone number to delete.</param>
internal record DeletePhoneCommand(Guid Id) : IRequest<Phone>;
