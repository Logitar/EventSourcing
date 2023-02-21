using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The command raised to create a new email address.
/// </summary>
/// <param name="Input">The creation input data.</param>
internal record CreateEmailCommand(CreateEmailInput Input) : IRequest<Email>;
