using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The query raised to retrieve a single postal address.
/// </summary>
/// <param name="Id">The identifier of the postal address.</param>
/// <param name="UserId">The identifier of the user to get the default postal address.</param>
internal record GetAddressQuery(Guid? Id, Guid? UserId) : IRequest<Address?>;
