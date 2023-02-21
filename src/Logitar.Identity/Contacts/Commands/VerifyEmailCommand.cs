using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to verify an existing email address.
/// </summary>
/// <param name="Id">The identifier of the email address to verify.</param>
internal record VerifyEmailCommand(Guid Id) : IRequest<Email>;
