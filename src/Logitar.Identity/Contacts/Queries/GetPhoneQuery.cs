using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The query raised to retrieve a single phone number.
/// </summary>
/// <param name="Id">The identifier of the phone number.</param>
/// <param name="UserId">The identifier of the user to get the default phone number.</param>
internal record GetPhoneQuery(Guid? Id, Guid? UserId) : IRequest<Phone?>;
