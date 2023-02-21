using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to update an existing phone number.
/// </summary>
/// <param name="Id">The identifier of the phone number to update.</param>
/// <param name="Input">The update input data.</param>
internal record UpdatePhoneCommand(Guid Id, UpdatePhoneInput Input) : IRequest<Phone>;
