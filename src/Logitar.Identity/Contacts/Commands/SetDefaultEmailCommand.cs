using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to set default an existing email address.
/// </summary>
/// <param name="Id">The identifier of the email address to set default.</param>
internal record SetDefaultEmailCommand(Guid Id) : IRequest<Email>;
