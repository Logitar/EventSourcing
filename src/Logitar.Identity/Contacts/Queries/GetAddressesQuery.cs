using MediatR;

namespace Logitar.Identity.Contacts.Queries;

/// <summary>
/// The query raised to retrieve a list of postal addresses.
/// </summary>
/// <param name="IsArchived">The value filtering postal addresses on their archivation status.</param>
/// <param name="IsVerified">The value filtering postal addresses on their verification status.</param>
/// <param name="Search">The text to search.</param>
/// <param name="UserId">The identifier of the user to filter by.</param>
/// <param name="Sort">The sort value.</param>
/// <param name="IsDescending">If true, the sort will be inverted.</param>
/// <param name="Skip">The number of postal addresses to skip.</param>
/// <param name="Take">The number of postal addresses to return.</param>
internal record GetAddressesQuery(bool? IsArchived, bool? IsVerified, string? Search, Guid? UserId,
  AddressSort? Sort, bool IsDescending, int? Skip, int? Take) : IRequest<PagedList<Address>>;
