using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to update an existing email address.
/// </summary>
/// <param name="Id">The identifier of the email address to update.</param>
/// <param name="Input">The update input data.</param>
internal record UpdateEmailCommand(Guid Id, UpdateEmailInput Input) : IRequest<Email>;
