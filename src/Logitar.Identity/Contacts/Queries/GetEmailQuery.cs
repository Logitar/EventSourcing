using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The query raised to retrieve a single email address.
/// </summary>
/// <param name="Id">The identifier of the email address.</param>
/// <param name="UserId">The identifier of the user to get the default email address.</param>
internal record GetEmailQuery(Guid? Id, Guid? UserId) : IRequest<Email?>;
