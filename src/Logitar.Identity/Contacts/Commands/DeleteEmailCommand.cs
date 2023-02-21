using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to delete an email address.
/// </summary>
/// <param name="Id">The identifier of the email address to delete.</param>
internal record DeleteEmailCommand(Guid Id) : IRequest<Email>;
